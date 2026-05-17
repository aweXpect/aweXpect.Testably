using System;
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
		where TFileSystem : class, IFileSystem
	{
		Func<TFileSystem, (IFileSystem fs, string fullPath)> resolver = fs => (fs, path);
		return new FileResult<TFileSystem>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasFileConstraint<TFileSystem>(it, grammars, path, resolver)),
			subject,
			path,
			resolver);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> does not have a file at the given <paramref name="path" />.
	/// </summary>
	public static AndOrResult<TFileSystem, IThat<TFileSystem>> DoesNotHaveFile<TFileSystem>(
		this IThat<TFileSystem> subject, string path)
		where TFileSystem : class, IFileSystem
	{
		Func<TFileSystem, (IFileSystem fs, string fullPath)> resolver = fs => (fs, path);
		return new AndOrResult<TFileSystem, IThat<TFileSystem>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasFileConstraint<TFileSystem>(it, grammars, path, resolver).Invert()),
			subject);
	}

	internal sealed class HasFileConstraint<TParent>(
		string it,
		ExpectationGrammars grammars,
		string path,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: ConstraintResult.WithValue<TParent>(grammars),
			IValueConstraint<TParent>
		where TParent : class
	{
		private IFileSystem? _fs;
		private string? _fullPath;

		public ConstraintResult IsMetBy(TParent actual)
		{
			Actual = actual;
			(_fs, _fullPath) = resolver(actual);
			Outcome = _fs.File.Exists(_fullPath) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has file '").Append(path).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_fs?.Directory.Exists(_fullPath) == true)
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
