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
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a <c>DateTime</c> read.
	/// </summary>
	public static TriggeredTimeSystemEventResult<DateTime> TriggeredDateTimeRead(
		this IThat<MockTimeSystem> subject)
		=> TriggeredDateTimeReadCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a <c>DateTime</c> read with a
	///     value matching the <paramref name="predicate" />.
	/// </summary>
	public static TriggeredTimeSystemEventResult<DateTime> TriggeredDateTimeRead(
		this IThat<MockTimeSystem> subject,
		Func<DateTime, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return TriggeredDateTimeReadCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a <c>DateTime</c> read.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<DateTime> DidNotTriggerDateTimeRead(
		this IThat<MockTimeSystem> subject)
		=> DidNotTriggerDateTimeReadCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a <c>DateTime</c>
	///     read for any value matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<DateTime> DidNotTriggerDateTimeRead(
		this IThat<MockTimeSystem> subject,
		Func<DateTime, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return DidNotTriggerDateTimeReadCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredTimeSystemEventResult<DateTime> TriggeredDateTimeReadCore(
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
					"triggered a DateTime read",
					"did not trigger a DateTime read",
					(ts, action, f) => ts.On.DateTimeRead(action, f),
					filter, quantifier, options, matches)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerTimeSystemEventResult<DateTime> DidNotTriggerDateTimeReadCore(
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
					"triggered a DateTime read",
					"did not trigger a DateTime read",
					(ts, action, f) => ts.On.DateTimeRead(action, f),
					filter, quantifier, options, matches,
					true).Invert()),
			subject,
			options,
			filter);
	}
}
