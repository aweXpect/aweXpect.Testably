using System.IO.Abstractions;
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
		=> new(subject.ThatIs().ExpectationBuilder.AddConstraint(it
				=> new HasDirectoryConstraint<TFileSystem>(it, path)),
			subject,
			path);

	private readonly struct HasDirectoryConstraint<TFileSystem>(string it, string path)
		: IValueConstraint<TFileSystem>
		where TFileSystem : IFileSystem
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			if (actual.Directory.Exists(path))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			if (actual.File.Exists(path))
			{
				return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
					$"{it} was a file");
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} did not exist");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"have directory '{path}'";
	}
}
