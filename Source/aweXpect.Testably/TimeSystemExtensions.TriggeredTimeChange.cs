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
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a time change.
	/// </summary>
	public static TriggeredTimeSystemEventResult<DateTime> TriggeredTimeChange(
		this IThat<MockTimeSystem> subject)
		=> TriggeredTimeChangeCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a time change to a value
	///     matching the <paramref name="predicate" />.
	/// </summary>
	public static TriggeredTimeSystemEventResult<DateTime> TriggeredTimeChange(
		this IThat<MockTimeSystem> subject,
		Func<DateTime, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return TriggeredTimeChangeCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a time change.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<DateTime> DidNotTriggerTimeChange(
		this IThat<MockTimeSystem> subject)
		=> DidNotTriggerTimeChangeCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a time change to
	///     any value matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<DateTime> DidNotTriggerTimeChange(
		this IThat<MockTimeSystem> subject,
		Func<DateTime, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return DidNotTriggerTimeChangeCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredTimeSystemEventResult<DateTime> TriggeredTimeChangeCore(
		IThat<MockTimeSystem> subject,
		Func<DateTime, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<DateTime> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<DateTime> matches = new();
		return new TriggeredTimeSystemEventResult<DateTime>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockTimeSystem, DateTime>(
					it, grammars,
					"triggered a time change",
					"did not trigger a time change",
					(ts, action, f) => ts.On.TimeChanged(action, d => f(d)),
					filter, quantifier, options, matches)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerTimeSystemEventResult<DateTime> DidNotTriggerTimeChangeCore(
		IThat<MockTimeSystem> subject,
		Func<DateTime, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<DateTime> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<DateTime> matches = new();
		return new DidNotTriggerTimeSystemEventResult<DateTime>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockTimeSystem, DateTime>(
					it, grammars,
					"triggered a time change",
					"did not trigger a time change",
					(ts, action, f) => ts.On.TimeChanged(action, d => f(d)),
					filter, quantifier, options, matches,
					true).Invert()),
			subject,
			options,
			filter);
	}
}
