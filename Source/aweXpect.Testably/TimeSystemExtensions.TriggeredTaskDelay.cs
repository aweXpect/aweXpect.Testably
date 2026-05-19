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
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a <c>Task.Delay</c>.
	/// </summary>
	public static TriggeredTimeSystemEventResult<TimeSpan> TriggeredTaskDelay(
		this IThat<MockTimeSystem> subject)
		=> TriggeredTaskDelayCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> triggered a <c>Task.Delay</c> with a
	///     value matching the <paramref name="predicate" />.
	/// </summary>
	public static TriggeredTimeSystemEventResult<TimeSpan> TriggeredTaskDelay(
		this IThat<MockTimeSystem> subject,
		Func<TimeSpan, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return TriggeredTaskDelayCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a <c>Task.Delay</c>.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<TimeSpan> DidNotTriggerTaskDelay(
		this IThat<MockTimeSystem> subject)
		=> DidNotTriggerTaskDelayCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockTimeSystem" /> did <i>not</i> trigger a <c>Task.Delay</c>
	///     for any value matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerTimeSystemEventResult<TimeSpan> DidNotTriggerTaskDelay(
		this IThat<MockTimeSystem> subject,
		Func<TimeSpan, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return DidNotTriggerTaskDelayCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredTimeSystemEventResult<TimeSpan> TriggeredTaskDelayCore(
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
					"triggered a Task.Delay",
					"did not trigger a Task.Delay",
					(ts, action, f) => ts.On.TaskDelay(action, f),
					filter, quantifier, options, matches)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerTimeSystemEventResult<TimeSpan> DidNotTriggerTaskDelayCore(
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
					"triggered a Task.Delay",
					"did not trigger a Task.Delay",
					(ts, action, f) => ts.On.TaskDelay(action, f),
					filter, quantifier, options, matches,
					true).Invert()),
			subject,
			options,
			filter);
	}
}
