using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" /> old path.
	/// </summary>
	/// <remarks>
	///     The old path is only set on a <see cref="System.IO.WatcherChangeTypes.Renamed" /> change.
	/// </remarks>
	public static StringEqualityTypeResult<ChangeDescription, IThat<ChangeDescription>> HasOldPath(
		this IThat<ChangeDescription> source,
		string? expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<ChangeDescription, IThat<ChangeDescription>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasStringPropertyConstraint(
					it, grammars, c => c.OldPath, options, expected, "old path")),
			source,
			options);
	}
}
