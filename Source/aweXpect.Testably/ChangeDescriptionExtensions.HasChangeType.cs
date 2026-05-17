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
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasChangeTypeConstraint(it, grammars, expected)),
			source);

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="WatcherChangeTypes" />.
	/// </summary>
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> DoesNotHaveChangeType(
		this IThat<ChangeDescription> source,
		WatcherChangeTypes unexpected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasChangeTypeConstraint(it, grammars, unexpected).Invert()),
			source);
}
