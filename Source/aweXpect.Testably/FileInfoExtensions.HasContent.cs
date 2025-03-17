using System.IO;
using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

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

	private sealed class HasContentValueConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<IFileInfo>(grammars),
			IValueConstraint<IFileInfo>
	{
		private string? _fileContent;

		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (!Actual.Exists)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			using StreamReader reader = actual.OpenText();
			_fileContent = reader.ReadToEnd();
			Outcome = options.AreConsideredEqual(_fileContent, expected) ? Outcome.Success : Outcome.Failure;
			expectationBuilder.UpdateContexts(contexts => contexts
				.Add(new ResultContext("File-Content", _fileContent)));
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Plural))
			{
				stringBuilder.Append("have Content ");
			}
			else
			{
				stringBuilder.Append("has Content ");
			}

			stringBuilder.Append(options.GetExpectation(expected, Grammars));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
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
	}
}
