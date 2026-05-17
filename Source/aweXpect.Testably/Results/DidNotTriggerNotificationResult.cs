using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="FileSystemExtensions.DidNotTriggerNotification(aweXpect.Core.IThat{MockFileSystem})" />.
/// </summary>
public class DidNotTriggerNotificationResult
	: AndOrResult<MockFileSystem, IThat<MockFileSystem>, DidNotTriggerNotificationResult>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly NotificationConstraints.TriggerNotificationFilter _filter;
	private readonly NotificationTimeoutOptions _options;

	internal DidNotTriggerNotificationResult(
		ExpectationBuilder expectationBuilder,
		IThat<MockFileSystem> subject,
		NotificationTimeoutOptions options,
		NotificationConstraints.TriggerNotificationFilter filter)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_options = options;
		_filter = filter;
	}

	/// <summary>
	///     Restricts the assertion to notifications that satisfy the inner <paramref name="expectation" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectation" /> is applied as an additional per-change filter, so any
	///     assertions from <see cref="ChangeDescriptionExtensions" /> (e.g. <c>.HasName(...)</c>,
	///     <c>.HasChangeType(...)</c>) compose naturally — the assertion fails if any notification
	///     satisfies all of them.
	/// </remarks>
	public DidNotTriggerNotificationResult Which(
		Action<IThat<ChangeDescription>> expectation,
		[CallerArgumentExpression("expectation")]
		string doNotPopulateThisValue = "")
	{
		if (expectation is null)
		{
			throw new ArgumentNullException(nameof(expectation));
		}

		ManualExpectationBuilder<ChangeDescription> manualBuilder =
			new(_expectationBuilder);
		expectation(new ThatSubject<ChangeDescription>(manualBuilder));
		_filter.Add(manualBuilder, doNotPopulateThisValue);
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous notifications.
	/// </summary>
	public DidNotTriggerNotificationResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
