using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a file.
/// </summary>
public partial class FileResult<TFileSystem>
{
	/// <summary>
	///     The result for additional verifications on a file content.
	/// </summary>
	public class Content(
		ExpectationBuilder expectationBuilder,
		FileResult<TFileSystem> subject,
		string path)
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> EqualTo(
			string expected)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((it, grammar)
					=> new HasStringContentConstraint(it, grammar, path, options, expected)),
				subject, options);
		}

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> DifferentFrom(
			string unexpected)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((it, grammar)
					=> new HasStringContentDifferentFromConstraint(it, path, options, unexpected)),
				subject, options);
		}
	}

	private readonly struct HasStringContentConstraint(
		string it,
		ExpectationGrammars grammar,
		string path,
		StringEqualityOptions options,
		string expected)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			string content = actual.File.ReadAllText(path);
			if (options.AreConsideredEqual(content, expected))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				options.GetExtendedFailure(it, content, expected));
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content {options.GetExpectation(expected, grammar)}";
	}

	private readonly struct HasStringContentDifferentFromConstraint(
		string it,
		string path,
		StringEqualityOptions options,
		string unexpected)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			string content = actual.File.ReadAllText(path);
			if (!options.AreConsideredEqual(content, unexpected))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}


			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} did match");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content different from {Formatter.Format(unexpected)}";
	}
}
