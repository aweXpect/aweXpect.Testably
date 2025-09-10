using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;

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
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasContentValueConstraint(
					expectationBuilder, it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the content of the <see cref="IFileInfo" /> is equal to
	///     the <paramref name="expected" /> binary content.
	/// </summary>
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> HasContent(this IThat<IFileInfo> source,
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> new(source.Get().ExpectationBuilder.AddConstraint((_, it, grammars)
				=> new HasBinaryContentConstraint(
					it, grammars, expected, doNotPopulateThisValue)),
			source);

	/// <summary>
	///     Verifies that the content of the <see cref="IFileInfo" />…
	/// </summary>
	public static FileInfoContentResult HasContent(this IThat<IFileInfo> source)
		=> new(source.Get().ExpectationBuilder, source);

	private sealed class HasContentValueConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<IFileInfo>(grammars),
			IAsyncConstraint<IFileInfo>
	{
		private string? _fileContent;

		public async Task<ConstraintResult> IsMetBy(IFileInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (!Actual.Exists)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			using StreamReader reader = actual.OpenText();
			_fileContent = await reader.ReadToEndAsync();
			Outcome = await options.AreConsideredEqual(_fileContent, expected) ? Outcome.Success : Outcome.Failure;
			expectationBuilder.UpdateContexts(contexts => contexts
				.Add(new ResultContext(Constants.FileContentContext, _fileContent)));
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Plural))
			{
				stringBuilder.Append("have content ");
			}
			else
			{
				stringBuilder.Append("has content ");
			}

			stringBuilder.Append(options.GetExpectation(expected, Grammars));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(it, Grammars, _fileContent, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class HasBinaryContentConstraint(
		string it,
		ExpectationGrammars grammars,
		byte[] expected,
		string expectedExpression)
		: ConstraintResult.WithValue<IFileInfo>(grammars),
			IValueConstraint<IFileInfo>
	{
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (!Actual.Exists)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			byte[] content = actual.FileSystem.File.ReadAllBytes(actual.FullName);
			Outcome = content.SequenceEqual(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Plural))
			{
				stringBuilder.Append("have content equal to ").Append(expectedExpression);
			}
			else
			{
				stringBuilder.Append("has content equal to ").Append(expectedExpression);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(it).Append(" differed");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Plural))
			{
				stringBuilder.Append("have content different from ").Append(expectedExpression);
			}
			else
			{
				stringBuilder.Append("has content different from ").Append(expectedExpression);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did match");
	}
}
