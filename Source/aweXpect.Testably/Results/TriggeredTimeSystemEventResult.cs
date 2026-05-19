using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for time-system trigger expectations on <see cref="MockTimeSystem" />.
/// </summary>
public class TriggeredTimeSystemEventResult<T>
	: CountResult<MockTimeSystem, IThat<MockTimeSystem>, TriggeredTimeSystemEventResult<T>>
	where T : notnull
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly NotificationConstraints.TriggerNotificationFilter<T> _filter;
	private readonly NotificationTimeoutOptions _options;

	internal TriggeredTimeSystemEventResult(
		ExpectationBuilder expectationBuilder,
		IThat<MockTimeSystem> subject,
		Quantifier quantifier,
		NotificationTimeoutOptions options,
		NotificationConstraints.TriggerNotificationFilter<T> filter)
		: base(expectationBuilder, subject, quantifier)
	{
		_expectationBuilder = expectationBuilder;
		_options = options;
		_filter = filter;
	}

	/// <summary>
	///     Restricts the assertion to events that satisfy the inner <paramref name="expectation" />.
	/// </summary>
	public TriggeredTimeSystemEventResult<T> Which(Action<IThat<T>> expectation)
	{
		if (expectation is null)
		{
			throw new ArgumentNullException(nameof(expectation));
		}

		ManualExpectationBuilder<T> manualBuilder = new(_expectationBuilder);
		expectation(new ThatSubject<T>(manualBuilder));
		_filter.Add(manualBuilder);
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous events.
	/// </summary>
	public TriggeredTimeSystemEventResult<T> Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
