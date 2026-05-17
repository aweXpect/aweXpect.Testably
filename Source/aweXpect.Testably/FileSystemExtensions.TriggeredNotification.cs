using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
	///     Verifies that executing the <paramref name="triggerAction" /> caused the
	///     <see cref="MockFileSystem.Notify" /> handler to fire.
	/// </summary>
	/// <remarks>
	///     Subscribes to <see cref="MockFileSystem.Notify" /> before invoking
	///     <paramref name="triggerAction" /> and counts notifications until either
	///     the configured timeout elapses or the matching count exceeds the upper
	///     bound implied by the quantifier. The default timeout is 30 seconds.
	/// </remarks>
	public static TriggeredNotificationResult TriggeredNotification(
		this IThat<MockFileSystem> subject,
		Func<CancellationToken, Task> triggerAction)
	{
		_ = triggerAction ?? throw new ArgumentNullException(nameof(triggerAction));
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter filter = new();
		List<ChangeDescription> captured = new();
		return new TriggeredNotificationResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint(
					it, grammars, triggerAction, filter, quantifier, options, captured)),
			subject,
			captured,
			filter,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that executing the <paramref name="triggerAction" /> caused the
	///     <see cref="MockFileSystem.Notify" /> handler to fire.
	/// </summary>
	public static TriggeredNotificationResult TriggeredNotification(
		this IThat<MockFileSystem> subject,
		Func<Task> triggerAction)
	{
		_ = triggerAction ?? throw new ArgumentNullException(nameof(triggerAction));
		return TriggeredNotification(subject, _ => triggerAction());
	}

	/// <summary>
	///     Verifies that executing the <paramref name="triggerAction" /> did <i>not</i> cause the
	///     <see cref="MockFileSystem.Notify" /> handler to fire.
	/// </summary>
	/// <remarks>
	///     Configure <c>.Within(...)</c> to limit the wait; otherwise the default 30-second
	///     timeout applies before the negative outcome is confirmed.
	/// </remarks>
	public static DidNotTriggerNotificationResult DidNotTriggerNotification(
		this IThat<MockFileSystem> subject,
		Func<CancellationToken, Task> triggerAction)
	{
		_ = triggerAction ?? throw new ArgumentNullException(nameof(triggerAction));
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter filter = new();
		List<ChangeDescription> captured = new();
		return new DidNotTriggerNotificationResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint(
					it, grammars, triggerAction, filter, quantifier, options, captured).Invert()),
			subject,
			captured,
			filter,
			options);
	}

	/// <summary>
	///     Verifies that executing the <paramref name="triggerAction" /> did <i>not</i> cause the
	///     <see cref="MockFileSystem.Notify" /> handler to fire.
	/// </summary>
	public static DidNotTriggerNotificationResult DidNotTriggerNotification(
		this IThat<MockFileSystem> subject,
		Func<Task> triggerAction)
	{
		_ = triggerAction ?? throw new ArgumentNullException(nameof(triggerAction));
		return DidNotTriggerNotification(subject, _ => triggerAction());
	}
}
