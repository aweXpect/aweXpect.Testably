using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" />
	///     <see cref="FileSystemTypes" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="FileSystemTypes" /> is a flag enum.
	/// </remarks>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> HasFileSystemType(
		this IThat<ChangeDescription> source,
		FileSystemTypes expected)
	{
		if (expected == default)
		{
			throw new ArgumentException(
				"The expected file system type must include at least one flag.", nameof(expected));
		}

		return new AndOrResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasFileSystemTypeConstraint(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileSystemTypes" />.
	/// </summary>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> DoesNotHaveFileSystemType(
		this IThat<ChangeDescription> source,
		FileSystemTypes unexpected)
	{
		if (unexpected == default)
		{
			throw new ArgumentException(
				"The unexpected file system type must include at least one flag.", nameof(unexpected));
		}

		return new AndOrResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasFileSystemTypeConstraint(it, grammars, unexpected).Invert()),
			source);
	}
}
