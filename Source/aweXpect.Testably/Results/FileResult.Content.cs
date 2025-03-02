using System.Linq;
using System.Runtime.CompilerServices;
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
		///     …is equal to the <paramref name="expected" /> binary.
		/// </summary>
		public AndOrResult<TFileSystem, FileResult<TFileSystem>> EqualTo(
			byte[] expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
			=> new(
				expectationBuilder.And(" ").AddConstraint((it, grammar)
					=> new HasBinaryContentEqualToConstraint(it, path, expected, doNotPopulateThisValue)),
				subject);

		/// <summary>
		///     …is equal to the <paramref name="expected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> EqualTo(
			string expected)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((expectationBuilder, it, grammar)
					=> new HasStringContentEqualToConstraint(expectationBuilder, it, grammar, path, options, expected)),
				subject, options);
		}

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> binary.
		/// </summary>
		public AndOrResult<TFileSystem, FileResult<TFileSystem>> NotEqualTo(
			byte[] unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
			=> new(
				expectationBuilder.And(" ").AddConstraint((it, grammar)
					=> new HasBinaryContentNotEqualToConstraint(it, path, unexpected, doNotPopulateThisValue)),
				subject);

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> NotEqualTo(
			string unexpected)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((expectationBuilder, it, grammar)
					=> new HasStringContentNotEqualToConstraint(expectationBuilder, it, path, options, unexpected)),
				subject, options);
		}

		/// <summary>
		///     …has the same content as the file on the <paramref name="filePath" />.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> SameAs(
			string filePath)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((expectationBuilder, it, grammar)
					=> new HasContentSameAsConstraint(expectationBuilder, it, path, options, filePath)),
				subject, options);
		}

		/// <summary>
		///     …does not have the same content as the file on the <paramref name="filePath" />.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> NotSameAs(
			string filePath)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((expectationBuilder, it, grammar)
					=> new HasContentNotSameAsConstraint(expectationBuilder, it, path, options, filePath)),
				subject, options);
		}
	}

	private readonly struct HasBinaryContentEqualToConstraint(
		string it,
		string path,
		byte[] expected,
		string expectedExpression)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			byte[] content = actual.File.ReadAllBytes(path);
			if (content.SequenceEqual(expected))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} differed");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content equal to {expectedExpression}";
	}

	private readonly struct HasBinaryContentNotEqualToConstraint(
		string it,
		string path,
		byte[] expected,
		string expectedExpression)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			byte[] content = actual.File.ReadAllBytes(path);
			if (!content.SequenceEqual(expected))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} did match");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content different from {expectedExpression}";
	}

	private readonly struct HasStringContentEqualToConstraint(
		ExpectationBuilder expectationBuilder,
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

			expectationBuilder.UpdateContexts(contexts => contexts
				.Add(new ResultContext("File content", content)));
			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				options.GetExtendedFailure(it, content, expected));
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content {options.GetExpectation(expected, grammar)}";
	}

	private readonly struct HasContentSameAsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string path,
		StringEqualityOptions options,
		string expectedPath)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			string actualContent = actual.File.ReadAllText(path);
			string fullPath = actual.Path.GetFullPath(expectedPath);
			if (!actual.File.Exists(expectedPath))
			{
				expectationBuilder.UpdateContexts(contexts => contexts
					.Add(new ResultContext("File content", actualContent)));
				return new ConstraintResult.Failure<TFileSystem>(actual, ToString(fullPath),
					$"{it} did not contain any file at '{fullPath}'");
			}

			string expectedContent = actual.File.ReadAllText(expectedPath);
			if (options.AreConsideredEqual(actualContent, expectedContent))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString(fullPath));
			}

			expectationBuilder.UpdateContexts(contexts => contexts
				.Add(new ResultContext("File content", actualContent)));
			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(fullPath),
				options.GetExtendedFailure(it, actualContent, expectedContent));
		}

		private static string ToString(string fullPath)
			=> $"with the same content as '{fullPath}'";
	}

	private readonly struct HasContentNotSameAsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		string path,
		StringEqualityOptions options,
		string expectedPath)
		: IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			string actualContent = actual.File.ReadAllText(path);
			string fullPath = actual.Path.GetFullPath(expectedPath);
			if (!actual.File.Exists(expectedPath))
			{
				expectationBuilder.UpdateContexts(contexts => contexts
					.Add(new ResultContext("File content", actualContent)));
				return new ConstraintResult.Failure<TFileSystem>(actual, ToString(fullPath),
					$"{it} did not contain any file at '{fullPath}'");
			}

			string expectedContent = actual.File.ReadAllText(expectedPath);
			if (!options.AreConsideredEqual(actualContent, expectedContent))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString(fullPath));
			}

			expectationBuilder.UpdateContexts(contexts => contexts
				.Add(new ResultContext("File content", actualContent)));
			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(fullPath),
				$"{it} did match");
		}

		private static string ToString(string fullPath)
			=> $"with not the same content as '{fullPath}'";
	}

	private readonly struct HasStringContentNotEqualToConstraint(
		ExpectationBuilder expectationBuilder,
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

			expectationBuilder.UpdateContexts(contexts => contexts
				.Add(new ResultContext("File content", content)));
			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} did match");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"with content different from {Formatter.Format(unexpected)}";
	}
}
