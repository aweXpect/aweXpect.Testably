using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

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
				expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasBinaryContentEqualToConstraint(
						it,
						grammars,
						path,
						expected,
						doNotPopulateThisValue)),
				subject);

		/// <summary>
		///     …is equal to the <paramref name="expected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> EqualTo(
			string expected)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((eb, it, grammars)
					=> new HasStringContentEqualToConstraint(
						eb,
						it,
						grammars,
						path,
						options,
						expected)),
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
				expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasBinaryContentEqualToConstraint(
							it,
							grammars,
							path,
							unexpected,
							doNotPopulateThisValue)
						.Invert()),
				subject);

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>> NotEqualTo(
			string unexpected)
		{
			StringEqualityOptions options = new();
			return new StringEqualityTypeResult<TFileSystem, FileResult<TFileSystem>>(
				expectationBuilder.And(" ").AddConstraint((eb, it, grammars)
					=> new HasStringContentEqualToConstraint(
						eb,
						it,
						grammars,
						path,
						options,
						unexpected).Invert()),
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
				expectationBuilder.And(" ").AddConstraint((eb, it, grammars)
					=> new HasContentSameAsConstraint(eb, it, grammars, path, options, filePath)),
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
				expectationBuilder.And(" ").AddConstraint((eb, it, grammars)
					=> new HasContentSameAsConstraint(eb, it, grammars, path, options, filePath).Invert()),
				subject, options);
		}
	}

	private sealed class HasBinaryContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		string path,
		byte[] expected,
		string expectedExpression)
		: ConstraintResult.WithValue<TFileSystem>(grammars),
			IValueConstraint<TFileSystem>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			byte[] content = actual.File.ReadAllBytes(path);
			Outcome = content.SequenceEqual(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("with content equal to ").Append(expectedExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" differed");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("with content different from ").Append(expectedExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did match");
	}

	private sealed class HasStringContentEqualToConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string path,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithValue<TFileSystem>(grammars),
			IValueConstraint<TFileSystem>
	{
		private string? _fileContent;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			_fileContent = actual.File.ReadAllText(path);
			Outcome = options.AreConsideredEqual(_fileContent, expected) ? Outcome.Success : Outcome.Failure;
			if (Outcome == Outcome.Failure)
			{
				expectationBuilder.UpdateContexts(contexts => contexts
					.Add(new ResultContext(Constants.FileContentContext, _fileContent)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("with content ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(it, Grammars, _fileContent, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("with content ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did match");
	}

	private sealed class HasContentSameAsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string path,
		StringEqualityOptions options,
		string expectedPath)
		: ConstraintResult.WithValue<TFileSystem>(grammars),
			IValueConstraint<TFileSystem>
	{
		private string? _expectedContent;
		private string? _fileContent;
		private string? _fullPath;
		private bool _isExpectedFound;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			_fileContent = actual.File.ReadAllText(path);
			_fullPath = actual.Path.GetFullPath(expectedPath);
			_isExpectedFound = actual.File.Exists(expectedPath);
			if (!_isExpectedFound)
			{
				expectationBuilder.UpdateContexts(contexts => contexts
					.Add(new ResultContext(Constants.FileContentContext, _fileContent)));
				Outcome = Outcome.Failure;
				return this;
			}

			_expectedContent = actual.File.ReadAllText(expectedPath);
			Outcome = options.AreConsideredEqual(_fileContent, _expectedContent) ? Outcome.Success : Outcome.Failure;
			if (Outcome == Outcome.Failure)
			{
				expectationBuilder.UpdateContexts(contexts => contexts
					.Add(new ResultContext(Constants.FileContentContext, _fileContent)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("with the same content as file '").Append(_fullPath).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_isExpectedFound)
			{
				stringBuilder.Append(it).Append(" did not contain any file at '").Append(_fullPath).Append('\'');
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(it, Grammars, _fileContent, _expectedContent));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("with not the same content as file '").Append(_fullPath).Append('\'');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_isExpectedFound)
			{
				stringBuilder.Append(it).Append(" did not contain any file at '").Append(_fullPath).Append('\'');
			}
			else
			{
				stringBuilder.Append(it).Append(" did match");
			}
		}
	}
}
