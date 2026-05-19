using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably.Helpers;

internal static class TimerConstraints
{
	internal sealed class TimerExecutedConstraint(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		NotificationTimeoutOptions options)
		: ConstraintResult.WithValue<ITimerMock>(grammars),
			IAsyncContextConstraint<ITimerMock>
	{
		private long _executionCount;

		public async Task<ConstraintResult> IsMetBy(ITimerMock actual,
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
			using CancellationTokenSource deadlineCts =
				CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			deadlineCts.CancelAfter(timeout);
			CancellationToken deadlineToken = deadlineCts.Token;

			TimeSpan pollInterval = TimeSpan.FromMilliseconds(10);
			while (true)
			{
				_executionCount = actual.ExecutionCount;
				if (quantifier.Check(ToInt(_executionCount), false) is not null)
				{
					break;
				}

				if (deadlineToken.IsCancellationRequested)
				{
					break;
				}

				try
				{
					await Task.Delay(pollInterval, deadlineToken).ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
					// deadline hit
				}
			}

			cancellationToken.ThrowIfCancellationRequested();

			_executionCount = actual.ExecutionCount;
			Outcome = quantifier.Check(ToInt(_executionCount), true) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		private static int ToInt(long value)
			=> value > int.MaxValue ? int.MaxValue : (int)value;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("executed");
			stringBuilder.Append(' ').Append(quantifier);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("did not execute");
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

			stringBuilder.Append(it).Append(" was ");
			if (_executionCount == 0)
			{
				stringBuilder.Append("not executed");
				return;
			}

			stringBuilder.Append("executed ");
			AppendTimes(stringBuilder, _executionCount);
		}

		private static void AppendTimes(StringBuilder stringBuilder, long count)
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
}
