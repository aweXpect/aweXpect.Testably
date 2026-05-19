using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably;

public static partial class TimeSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a <c>Thread.Sleep</c>.
	/// </summary>
	public static TriggeredTimeSystemEventResult<TimeSpan> TriggeredThreadSleep(
		this IThat<MockTimeSystem> subject)
		=> TriggeredThreadSleepCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a <c>Thread.Sleep</c> with a
	///     value matching the <paramref name="predicate" />.
	/// </summary>
	public static TriggeredTimeSystemEventResult<TimeSpan> TriggeredThreadSleep(
		this IThat<MockTimeSystem> subject,
		Func<TimeSpan, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return TriggeredThreadSleepCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a <c>Thread.Sleep</c>.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<TimeSpan> DidNotTriggerThreadSleep(
		this IThat<MockTimeSystem> subject)
		=> DidNotTriggerThreadSleepCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a <c>Thread.Sleep</c>
	///     for any value matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<TimeSpan> DidNotTriggerThreadSleep(
		this IThat<MockTimeSystem> subject,
		Func<TimeSpan, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return DidNotTriggerThreadSleepCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredTimeSystemEventResult<TimeSpan> TriggeredThreadSleepCore(
		IThat<MockTimeSystem> subject,
		Func<TimeSpan, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<TimeSpan> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<TimeSpan> matches = new();
		return new TriggeredTimeSystemEventResult<TimeSpan>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockTimeSystem, TimeSpan>(
					it, grammars,
					"triggered a Thread.Sleep",
					"did not trigger a Thread.Sleep",
					(ts, action, f) => ts.On.ThreadSleep(action, f),
					filter, quantifier, options, matches)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerTimeSystemEventResult<TimeSpan> DidNotTriggerThreadSleepCore(
		IThat<MockTimeSystem> subject,
		Func<TimeSpan, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<TimeSpan> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<TimeSpan> matches = new();
		return new DidNotTriggerTimeSystemEventResult<TimeSpan>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockTimeSystem, TimeSpan>(
					it, grammars,
					"triggered a Thread.Sleep",
					"did not trigger a Thread.Sleep",
					(ts, action, f) => ts.On.ThreadSleep(action, f),
					filter, quantifier, options, matches,
					true).Invert()),
			subject,
			options,
			filter);
	}
}
