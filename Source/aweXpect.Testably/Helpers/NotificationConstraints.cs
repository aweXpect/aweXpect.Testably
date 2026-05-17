using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Helpers;

internal static class NotificationConstraints
{
	internal sealed class TriggeredNotificationConstraint(
		string it,
		ExpectationGrammars grammars,
		TriggerNotificationFilter filter,
		Quantifier quantifier,
		NotificationTimeoutOptions options,
		List<ChangeDescription> matches,
		bool exitOnFirstMatch = false)
		: ConstraintResult.WithValue<MockFileSystem>(grammars),
			IAsyncContextConstraint<MockFileSystem>
	{
		public async Task<ConstraintResult> IsMetBy(MockFileSystem actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
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

			using IAwaitableCallback<ChangeDescription> registration = actual.Notify.OnEventOrReplay(
				change =>
				{
					if (!filter.IsAsyncMatchSync(change, context, cancellationToken))
					{
						return;
					}

					lock (matches)
					{
						if (exitOnFirstMatch && matches.Count > 0)
						{
							return;
						}

						matches.Add(change);
						if (exitOnFirstMatch || quantifier.Check(matches.Count, false) is not null)
						{
							earlyExit.TrySetResult(false);
						}
					}
				},
				filter.IsSyncMatch);

			if (!deadlineToken.IsCancellationRequested && !earlyExit.Task.IsCompleted)
			{
				await Task.WhenAny(
					earlyExit.Task,
					Task.Delay(Timeout.InfiniteTimeSpan, deadlineToken)).ConfigureAwait(false);
			}

			cancellationToken.ThrowIfCancellationRequested();

			int matchCount;
			lock (matches)
			{
				matchCount = matches.Count;
			}

			Outcome = quantifier.Check(matchCount, true) == true ? Outcome.Success : Outcome.Failure;
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
			=> AppendCount(stringBuilder);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("did not trigger a notification");
			stringBuilder.Append(filter);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder);

		private void AppendCount(StringBuilder stringBuilder)
		{
			if (Actual is null)
			{
				stringBuilder.Append(it).Append(" was <null>");
				return;
			}

			ChangeDescription[] snapshot;
			lock (matches)
			{
				snapshot = matches.ToArray();
			}

			stringBuilder.Append(it).Append(" was ");
			if (snapshot.Length == 0)
			{
				stringBuilder.Append("not triggered");
				return;
			}

			stringBuilder.Append("triggered ");
			AppendTimes(stringBuilder, snapshot.Length);
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
		private readonly List<ManualExpectationBuilder<ChangeDescription>> _asyncFilters = new();
		private readonly List<(Func<ChangeDescription, bool> Predicate, string Description)> _syncPredicates = new();

		public void Add(Func<ChangeDescription, bool> predicate, string predicateExpression)
		{
			if (predicate is null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			_syncPredicates.Add((predicate, predicateExpression.Trim()));
		}

		public void Add(ManualExpectationBuilder<ChangeDescription> builder)
			=> _asyncFilters.Add(builder);

		public bool IsSyncMatch(ChangeDescription change)
			=> _syncPredicates.All(p => p.Predicate(change));

		public bool IsAsyncMatchSync(ChangeDescription change,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			foreach (ManualExpectationBuilder<ChangeDescription> builder in _asyncFilters)
			{
				ConstraintResult result = builder.IsMetBy(change, context, cancellationToken)
					.ConfigureAwait(false).GetAwaiter().GetResult();
				if (result.Outcome != Outcome.Success)
				{
					return false;
				}
			}

			return true;
		}

		public override string ToString()
		{
			if (_syncPredicates.Count == 0 && _asyncFilters.Count == 0)
			{
				return "";
			}

			StringBuilder sb = new();
			bool firstInGroup = true;
			foreach ((Func<ChangeDescription, bool> _, string description) in _syncPredicates)
			{
				sb.Append(firstInGroup ? " matching " : " and ").Append(description);
				firstInGroup = false;
			}

			firstInGroup = true;
			foreach (ManualExpectationBuilder<ChangeDescription> builder in _asyncFilters)
			{
				sb.Append(firstInGroup ? " which " : " and ");
				builder.AppendExpectation(sb, indentation: "");
				firstInGroup = false;
			}

			return sb.ToString();
		}
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
