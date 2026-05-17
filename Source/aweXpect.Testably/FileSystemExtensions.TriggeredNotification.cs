using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class FileSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler fired.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="INotificationHandler.OnEventOrReplay" /> so notifications
	///     that already fired on this <see cref="MockFileSystem" /> count toward the quantifier.
	///     Combine with <c>.Within(...)</c> to also wait for asynchronous notifications.
	/// </remarks>
	public static TriggeredNotificationResult TriggeredNotification(
		this IThat<MockFileSystem> subject)
		=> TriggeredNotificationCore(subject, predicate: null, predicateExpression: "");

	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler fired for a change
	///     matching the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="INotificationHandler.OnEventOrReplay" /> so notifications
	///     that already fired on this <see cref="MockFileSystem" /> count toward the quantifier.
	/// </remarks>
	public static TriggeredNotificationResult TriggeredNotification(
		this IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
	{
		_ = predicate ?? throw new ArgumentNullException(nameof(predicate));
		return TriggeredNotificationCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler did <i>not</i> fire.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="INotificationHandler.OnEventOrReplay" /> so any notification
	///     that already fired on this <see cref="MockFileSystem" /> fails the assertion. Configure
	///     <c>.Within(...)</c> to also wait for asynchronous notifications.
	/// </remarks>
	public static DidNotTriggerNotificationResult DidNotTriggerNotification(
		this IThat<MockFileSystem> subject)
		=> DidNotTriggerNotificationCore(subject, predicate: null, predicateExpression: "");

	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler did <i>not</i> fire for
	///     any change matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerNotificationResult DidNotTriggerNotification(
		this IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
	{
		_ = predicate ?? throw new ArgumentNullException(nameof(predicate));
		return DidNotTriggerNotificationCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredNotificationResult TriggeredNotificationCore(
		IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<ChangeDescription> captured = new();
		return new TriggeredNotificationResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint(
					it, grammars, filter, quantifier, options, captured)),
			subject,
			captured,
			filter,
			quantifier,
			options);
	}

	private static DidNotTriggerNotificationResult DidNotTriggerNotificationCore(
		IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		List<ChangeDescription> captured = new();
		return new DidNotTriggerNotificationResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint(
					it, grammars, filter, quantifier, options, captured,
					exitOnFirstMatch: true).Invert()),
			subject,
			captured,
			filter,
			options);
	}
}
