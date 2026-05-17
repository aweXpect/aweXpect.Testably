using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> exists.
	/// </summary>
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> Exists(this IThat<IDirectoryInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ExistsConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> does not exist.
	/// </summary>
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> DoesNotExist(this IThat<IDirectoryInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ExistsConstraint(it, grammars).Invert()),
			source);

	private sealed class ExistsConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IDirectoryInfo>(grammars),
			IValueConstraint<IDirectoryInfo>
	{
		public ConstraintResult IsMetBy(IDirectoryInfo actual)
		{
			Actual = actual;
			Outcome = actual.Exists ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("exists");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not exist");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");
	}
}
