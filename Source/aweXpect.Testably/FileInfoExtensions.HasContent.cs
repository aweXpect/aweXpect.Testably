using System.IO;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the string content of the <see cref="IFileInfo" /> is equal to
	///     the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> HasContent(this IThat<IFileInfo> source,
		string? expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar)
				=> new HasContentValueConstraint(
					it, "have", expected, options)),
			source,
			options);
	}

	private readonly struct HasContentValueConstraint(
		string it,
		string verb,
		string? expected,
		StringEqualityOptions options)
		: IValueConstraint<IFileInfo>
	{
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			using StreamReader reader = actual.OpenText();
			string content = reader.ReadToEnd();
			if (options.AreConsideredEqual(content, expected))
			{
				return new ConstraintResult.Success<IFileInfo>(actual, ToString());
			}

			return new ConstraintResult.Failure<IFileInfo?>(actual, ToString(),
				options.GetExtendedFailure(it, expected, content));
		}

		public override string ToString()
			=> $"{verb} Content {options.GetExpectation(expected, ExpectationGrammars.None)}{options}";
	}
}
