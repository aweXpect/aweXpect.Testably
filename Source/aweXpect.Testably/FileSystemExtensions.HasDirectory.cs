using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Testably.Results;

namespace aweXpect.Testably;

public static partial class FileSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> has a directory at the given <paramref name="path" />.
	/// </summary>
	public static DirectoryResult<TFileSystem> HasDirectory<TFileSystem>(
		this IThat<TFileSystem> subject, string path)
		where TFileSystem : IFileSystem
		=> new(subject.ThatIs().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasDirectoryConstraint<TFileSystem>(it, grammars, path)),
			subject,
			path);

	private sealed class HasDirectoryConstraint<TFileSystem>(string it, ExpectationGrammars grammars, string path)
		: ConstraintResult.WithValue<TFileSystem>(grammars),
			IValueConstraint<TFileSystem>
		where TFileSystem : IFileSystem
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			Actual = actual;
			Outcome = actual.Directory.Exists(path) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has directory '").Append(path).Append("'");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.File.Exists(path) == true)
			{
				stringBuilder.Append(it).Append(" was a file");
			}
			else
			{
				stringBuilder.Append(it).Append(" did not exist");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have directory '").Append(path).Append("'");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" had");
	}
}
