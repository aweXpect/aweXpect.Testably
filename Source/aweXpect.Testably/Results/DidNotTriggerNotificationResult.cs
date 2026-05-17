using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="FileSystemExtensions.DidNotTriggerNotification(IThat{MockFileSystem}, Func{Task})" />.
/// </summary>
public class DidNotTriggerNotificationResult
	: AndOrResult<MockFileSystem, IThat<MockFileSystem>, DidNotTriggerNotificationResult>
{
	private readonly IReadOnlyList<ChangeDescription> _captured;
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly NotificationConstraints.TriggerNotificationFilter _filter;
	private readonly NotificationTimeoutOptions _options;

	internal DidNotTriggerNotificationResult(
		ExpectationBuilder expectationBuilder,
		IThat<MockFileSystem> subject,
		IReadOnlyList<ChangeDescription> captured,
		NotificationConstraints.TriggerNotificationFilter filter,
		NotificationTimeoutOptions options)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_captured = captured;
		_filter = filter;
		_options = options;
	}

	/// <summary>
	///     Further expectations on the list of <see cref="ChangeDescription" />s captured during the trigger action.
	/// </summary>
	public IThat<IReadOnlyList<ChangeDescription>> Which
		=> new ThatSubject<IReadOnlyList<ChangeDescription>>(
			_expectationBuilder.ForWhich<MockFileSystem, IReadOnlyList<ChangeDescription>>(
				_ => _captured, " which "));

	/// <summary>
	///     Restricts the trigger expectation to notifications matching the <paramref name="predicate" />.
	/// </summary>
	public DidNotTriggerNotificationResult Matching(
		Func<ChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		_filter.Add(predicate, doNotPopulateThisValue);
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> for the trigger action plus subsequent notifications.
	/// </summary>
	public DidNotTriggerNotificationResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
