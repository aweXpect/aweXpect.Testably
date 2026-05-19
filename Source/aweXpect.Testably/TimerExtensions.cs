using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="ITimerMock" />.
/// </summary>
public static class TimerExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ITimerMock" /> callback was executed.
	/// </summary>
	/// <remarks>
	///     Polls <see cref="ITimerMock.ExecutionCount" /> until either the quantifier is satisfied
	///     or the timeout expires — 30 seconds by default; use <c>.Within(timeout)</c> to override.
	/// </remarks>
	public static TimerExecutedResult Executed(
		this IThat<ITimerMock> subject)
		=> ExecutedCore(subject, null);

	/// <summary>
	///     Verifies that the <see cref="ITimerMock" /> callback was executed exactly
	///     <paramref name="times" />.
	/// </summary>
	/// <remarks>
	///     Convenience for <c>.Executed().Exactly(times)</c>. Polls
	///     <see cref="ITimerMock.ExecutionCount" /> until either the quantifier is satisfied or the
	///     timeout expires — 30 seconds by default; use <c>.Within(timeout)</c> to override.
	/// </remarks>
	public static TimerExecutedResult Executed(
		this IThat<ITimerMock> subject,
		Times times)
		=> ExecutedCore(subject, times);

	private static TimerExecutedResult ExecutedCore(
		IThat<ITimerMock> subject,
		Times? times)
	{
		Quantifier quantifier = new();
		if (times.HasValue)
		{
			quantifier.Exactly(times.Value.Value);
		}

		NotificationTimeoutOptions options = new();
		return new TimerExecutedResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new TimerConstraints.TimerExecutedConstraint(it, grammars, quantifier, options)),
			subject,
			quantifier,
			options);
	}
}
