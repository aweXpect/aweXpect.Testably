using System;
using System.IO;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" />
	///     <see cref="NotifyFilters" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="NotifyFilters" /> is a flag enum.
	/// </remarks>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> HasNotifyFilters(
		this IThat<ChangeDescription> source,
		NotifyFilters expected)
	{
		if (expected == default)
		{
			throw new ArgumentException(
				"The expected notify filters must include at least one flag.", nameof(expected));
		}

		return new AndOrResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasNotifyFiltersConstraint(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="NotifyFilters" />.
	/// </summary>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> DoesNotHaveNotifyFilters(
		this IThat<ChangeDescription> source,
		NotifyFilters unexpected)
	{
		if (unexpected == default)
		{
			throw new ArgumentException(
				"The unexpected notify filters must include at least one flag.", nameof(unexpected));
		}

		return new AndOrResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasNotifyFiltersConstraint(it, grammars, unexpected).Invert()),
			source);
	}
}
