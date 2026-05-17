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
	///     <see cref="WatcherChangeTypes" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="WatcherChangeTypes" /> is a flag enum.
	/// </remarks>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> HasChangeType(
		this IThat<ChangeDescription> source,
		WatcherChangeTypes expected)
	{
		if (expected == default)
		{
			throw new ArgumentException(
				"The expected change type must include at least one flag.", nameof(expected));
		}

		return new AndOrResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasChangeTypeConstraint(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="WatcherChangeTypes" />.
	/// </summary>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> DoesNotHaveChangeType(
		this IThat<ChangeDescription> source,
		WatcherChangeTypes unexpected)
	{
		if (unexpected == default)
		{
			throw new ArgumentException(
				"The unexpected change type must include at least one flag.", nameof(unexpected));
		}

		return new AndOrResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasChangeTypeConstraint(it, grammars, unexpected).Invert()),
			source);
	}
}
