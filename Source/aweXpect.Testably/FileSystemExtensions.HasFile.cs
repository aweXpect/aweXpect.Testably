using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;

namespace aweXpect.Testably;

public static partial class FileSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> has a file at the given <paramref name="path" />.
	/// </summary>
	public static FileResult<TFileSystem> HasFile<TFileSystem>(
		this IThat<TFileSystem> subject, string path)
		where TFileSystem : IFileSystem
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasFileConstraint<TFileSystem>(it, grammars, path)),
			subject,
			path);

	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> does not have a file at the given <paramref name="path" />.
	/// </summary>
	public static AndOrResult<TFileSystem, IThat<TFileSystem>> DoesNotHaveFile<TFileSystem>(
		this IThat<TFileSystem> subject, string path)
		where TFileSystem : IFileSystem
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasFileConstraint<TFileSystem>(it, grammars, path).Invert()),
			subject);

	private sealed class HasFileConstraint<TFileSystem>(string it, ExpectationGrammars grammars, string path)
		: ConstraintResult.WithValue<TFileSystem>(grammars),
			IValueConstraint<TFileSystem>
		where TFileSystem : IFileSystem
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			Actual = actual;
			Outcome = actual.File.Exists(path) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has file '").Append(path).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Directory.Exists(path) == true)
			{
				stringBuilder.Append(it).Append(" was a directory");
			}
			else
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have file '").Append(path).Append('\'');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" did");
	}
}
