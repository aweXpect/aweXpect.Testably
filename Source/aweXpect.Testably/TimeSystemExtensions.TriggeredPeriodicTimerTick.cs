#if FEATURE_PERIODIC_TIMER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing;
using Testably.Abstractions.TimeSystem;

namespace aweXpect.Testably;

public static partial class TimeSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a periodic timer tick wait.
	/// </summary>
	public static TriggeredTimeSystemEventResult<IPeriodicTimer> TriggeredPeriodicTimerTick(
		this IThat<MockTimeSystem> subject)
		=> TriggeredPeriodicTimerTickCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a periodic timer tick wait for
	///     a timer matching the <paramref name="predicate" />.
	/// </summary>
	public static TriggeredTimeSystemEventResult<IPeriodicTimer> TriggeredPeriodicTimerTick(
		this IThat<MockTimeSystem> subject,
		Func<IPeriodicTimer, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return TriggeredPeriodicTimerTickCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a periodic timer
	///     tick wait.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<IPeriodicTimer> DidNotTriggerPeriodicTimerTick(
		this IThat<MockTimeSystem> subject)
		=> DidNotTriggerPeriodicTimerTickCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a periodic timer
	///     tick wait for any timer matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<IPeriodicTimer> DidNotTriggerPeriodicTimerTick(
		this IThat<MockTimeSystem> subject,
		Func<IPeriodicTimer, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return DidNotTriggerPeriodicTimerTickCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredTimeSystemEventResult<IPeriodicTimer> TriggeredPeriodicTimerTickCore(
		IThat<MockTimeSystem> subject,
		Func<IPeriodicTimer, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<IPeriodicTimer> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<IPeriodicTimer> matches = new();
		return new TriggeredTimeSystemEventResult<IPeriodicTimer>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockTimeSystem, IPeriodicTimer>(
					it, grammars,
					"triggered a periodic timer tick",
					"did not trigger a periodic timer tick",
					(ts, action, f) => ts.On.PeriodicTimer.WaitingForNextTick(action, f),
					filter, quantifier, options, matches)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerTimeSystemEventResult<IPeriodicTimer> DidNotTriggerPeriodicTimerTickCore(
		IThat<MockTimeSystem> subject,
		Func<IPeriodicTimer, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<IPeriodicTimer> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<IPeriodicTimer> matches = new();
		return new DidNotTriggerTimeSystemEventResult<IPeriodicTimer>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockTimeSystem, IPeriodicTimer>(
					it, grammars,
					"triggered a periodic timer tick",
					"did not trigger a periodic timer tick",
					(ts, action, f) => ts.On.PeriodicTimer.WaitingForNextTick(action, f),
					filter, quantifier, options, matches,
					true).Invert()),
			subject,
			options,
			filter);
	}
}
#endif
