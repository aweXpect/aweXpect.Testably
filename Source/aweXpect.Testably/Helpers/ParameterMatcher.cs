using System;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Helpers;

internal sealed class ParameterMatcher
{
	private readonly Func<ParameterDescription, bool>? _matcher;

	private ParameterMatcher(string name, Func<ParameterDescription, bool>? matcher, string? expression)
	{
		Name = name;
		Expression = expression;
		_matcher = matcher;
	}

	public string Name { get; }

	public string? Expression { get; }

	public bool IsAny => _matcher is null;

	public static ParameterMatcher From<T>(string name, Func<T, bool>? predicate, string? expression)
		=> predicate is null
			? new ParameterMatcher(name, null, null)
			: new ParameterMatcher(name, p => p.Is(predicate), expression);

	public bool IsMatch(ParameterDescription parameter)
		=> _matcher is null || _matcher(parameter);
}
