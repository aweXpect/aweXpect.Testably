using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
		=> new(subject.ThatIs().ExpectationBuilder.AddConstraint(it
				=> new HasFileConstraint<TFileSystem>(it, path)),
			subject,
			path);

	private readonly struct HasFileConstraint<TFileSystem>(string it, string path)
		: IValueConstraint<TFileSystem>
		where TFileSystem : IFileSystem
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			if (actual.File.Exists(path))
			{
				return new ConstraintResult.Success<TFileSystem>(actual, ToString());
			}

			if (actual.Directory.Exists(path))
			{
				return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
					$"{it} was a directory");
			}

			return new ConstraintResult.Failure<TFileSystem>(actual, ToString(),
				$"{it} did not exist");
		}

		/// <inheritdoc />
		public override string ToString()
			=> $"have file '{path}'";
	}
}
