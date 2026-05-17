using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Helpers;

internal static class NotificationConstraints
{
	internal sealed class TriggeredNotificationConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<CancellationToken, Task> action,
		TriggerNotificationFilter filter,
		Quantifier quantifier,
		NotificationTimeoutOptions options,
		List<ChangeDescription> captured,
		bool exitOnFirstMatch = false)
		: ConstraintResult.WithValue<MockFileSystem>(grammars),
			IAsyncConstraint<MockFileSystem>
	{
		private Exception? _actionException;

		public async Task<ConstraintResult> IsMetBy(MockFileSystem actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			TimeSpan timeout = options.Timeout;
			TaskCompletionSource<bool> earlyExit = new();

			using CancellationTokenSource deadlineCts =
				CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			deadlineCts.CancelAfter(timeout);
			CancellationToken deadlineToken = deadlineCts.Token;

			using IAwaitableCallback<ChangeDescription> registration = actual.Notify.OnEvent(
				change =>
				{
					lock (captured)
					{
						captured.Add(change);
						if (exitOnFirstMatch || quantifier.Check(captured.Count, false) == false)
						{
							earlyExit.TrySetResult(false);
						}
					}
				},
				filter.IsMatch);

			try
			{
				await action(deadlineToken).ConfigureAwait(false);
			}
			catch (OperationCanceledException) when (deadlineToken.IsCancellationRequested)
			{
				// Deadline expired during the trigger action; evaluate what was captured so far.
			}
			catch (Exception ex)
			{
				_actionException = ex;
				Outcome = Outcome.Failure;
				return this;
			}

			if (!deadlineToken.IsCancellationRequested && !earlyExit.Task.IsCompleted)
			{
				await Task.WhenAny(
					earlyExit.Task,
					Task.Delay(Timeout.InfiniteTimeSpan, deadlineToken)).ConfigureAwait(false);
			}

			int finalCount;
			lock (captured)
			{
				finalCount = captured.Count;
			}

			Outcome = quantifier.Check(finalCount, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("triggered a notification");
			stringBuilder.Append(filter);
			stringBuilder.Append(' ').Append(quantifier);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder, indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("did not trigger a notification");
			stringBuilder.Append(filter);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder, indentation);

		private void AppendCount(StringBuilder stringBuilder, string? indentation)
		{
			if (_actionException is not null)
			{
				stringBuilder.Append(it).Append(" threw ");
				Formatter.Format(stringBuilder, _actionException.GetType());
				stringBuilder.Append(": ").Append(_actionException.Message);
				return;
			}

			int count;
			ChangeDescription[] snapshot;
			lock (captured)
			{
				count = captured.Count;
				snapshot = captured.ToArray();
			}

			stringBuilder.Append(it).Append(" was ");
			if (count == 0)
			{
				stringBuilder.Append("not triggered");
				return;
			}

			stringBuilder.Append("triggered ");
			AppendTimes(stringBuilder, count);
			stringBuilder.Append(" in [");
			for (int i = 0; i < snapshot.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}

				stringBuilder.Append(Environment.NewLine).Append("  ").Append(snapshot[i]);
			}

			stringBuilder.Append(Environment.NewLine).Append(']');
		}

		private static void AppendTimes(StringBuilder stringBuilder, int count)
		{
			switch (count)
			{
				case 1:
					stringBuilder.Append("once");
					break;
				case 2:
					stringBuilder.Append("twice");
					break;
				default:
					stringBuilder.Append(count).Append(" times");
					break;
			}
		}
	}

	internal sealed class TriggerNotificationFilter
	{
		private readonly List<Func<ChangeDescription, bool>> _predicates = new();
		private readonly StringBuilder _toString = new();

		public void Add(Func<ChangeDescription, bool> predicate, string predicateExpression)
		{
			_predicates.Add(predicate ?? throw new ArgumentNullException(nameof(predicate)));
			_toString.Append(_predicates.Count == 1 ? " matching " : " and ").Append(predicateExpression.Trim());
		}

		public bool IsMatch(ChangeDescription change)
			=> _predicates.All(p => p(change));

		public override string ToString() => _toString.ToString();
	}

	internal sealed class HasChangeTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		WatcherChangeTypes expected)
		: ConstraintResult.WithValue<ChangeDescription>(grammars),
			IValueConstraint<ChangeDescription>
	{
		public ConstraintResult IsMetBy(ChangeDescription actual)
		{
			Actual = actual;
			Outcome = actual != null! && (actual.ChangeType & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has change type ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" was ").Append(Actual.ChangeType);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have change type ").Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}

	internal sealed class HasFileSystemTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		FileSystemTypes expected)
		: ConstraintResult.WithValue<ChangeDescription>(grammars),
			IValueConstraint<ChangeDescription>
	{
		public ConstraintResult IsMetBy(ChangeDescription actual)
		{
			Actual = actual;
			Outcome = actual != null! && (actual.FileSystemType & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has file system type ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" was ").Append(Actual.FileSystemType);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have file system type ").Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}

	internal sealed class HasNotifyFiltersConstraint(
		string it,
		ExpectationGrammars grammars,
		NotifyFilters expected)
		: ConstraintResult.WithValue<ChangeDescription>(grammars),
			IValueConstraint<ChangeDescription>
	{
		public ConstraintResult IsMetBy(ChangeDescription actual)
		{
			Actual = actual;
			Outcome = actual != null! && (actual.NotifyFilters & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has notify filters ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" was ").Append(Actual.NotifyFilters);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have notify filters ").Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}

	internal sealed class HasStringPropertyConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<ChangeDescription, string?> selector,
		StringEqualityOptions options,
		string? expected,
		string propertyName)
		: ConstraintResult.WithValue<ChangeDescription>(grammars),
			IAsyncConstraint<ChangeDescription>
	{
		private string? _actualValue;

		public async Task<ConstraintResult> IsMetBy(ChangeDescription actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_actualValue = selector(actual);
			Outcome = await options.AreConsideredEqual(_actualValue, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(propertyName).Append(' ')
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(it, Grammars, _actualValue, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have ").Append(propertyName).Append(' ')
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}
}
