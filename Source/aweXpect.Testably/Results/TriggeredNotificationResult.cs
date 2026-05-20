using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="FileSystemExtensions.TriggeredNotification(aweXpect.Core.IThat{MockFileSystem})" />.
/// </summary>
public class TriggeredNotificationResult
	: CountResult<MockFileSystem, IThat<MockFileSystem>, TriggeredNotificationResult>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly NotificationConstraints.TriggerNotificationFilter<ChangeDescription> _filter;
	private readonly NotificationTimeoutOptions _options;

	internal TriggeredNotificationResult(
		ExpectationBuilder expectationBuilder,
		IThat<MockFileSystem> subject,
		Quantifier quantifier,
		NotificationTimeoutOptions options, NotificationConstraints.TriggerNotificationFilter<ChangeDescription> filter)
		: base(expectationBuilder, subject, quantifier)
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
	///     <c>.HasChangeType(...)</c>) compose naturally (only notifications that satisfy all of
	///     them count toward the quantifier). The expectation text is taken from the inner
	///     expectation builder, so it reads like <c>matching has name equal to "foo.txt"</c> rather
	///     than the raw lambda source.
	/// </remarks>
	public TriggeredNotificationResult Which(Action<IThat<ChangeDescription>> expectation)
	{
		if (expectation is null)
		{
			throw new ArgumentNullException(nameof(expectation));
		}

		ManualExpectationBuilder<ChangeDescription> manualBuilder = new(_expectationBuilder);
		expectation(new ThatSubject<ChangeDescription>(manualBuilder));
		_filter.Add(manualBuilder);
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous notifications.
	/// </summary>
	public TriggeredNotificationResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
