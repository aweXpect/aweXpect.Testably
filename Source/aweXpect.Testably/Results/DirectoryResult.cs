using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a directory.
/// </summary>
public class DirectoryResult<TFileSystem>(
	ExpectationBuilder expectationBuilder,
	IThat<TFileSystem> subject,
	string path)
	: AndOrResult<IFileSystem, IThat<TFileSystem>>(expectationBuilder, subject)
	where TFileSystem : IFileSystem
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Verifies that the files of the directory satisfy the <paramref name="expectations" />.
	/// </summary>
	public DirectoryResult<TFileSystem> WithFiles(
		Action<IThat<IEnumerable<IFileInfo>>> expectations)
	{
		_expectationBuilder
			.ForMember(
				MemberAccessor<TFileSystem, IEnumerable<IFileInfo>>.FromFunc(
					f => f.Directory.EnumerateFiles(path).Select(p => f.FileInfo.New(p)), "files "),
				(property, expectation) => $" which {property}should {expectation}")
			.AddExpectations(e
				=> expectations(new ThatSubject<IEnumerable<IFileInfo>>(e)));
		return this;
	}

	/// <summary>
	///     Verifies that the subdirectories of the directory satisfy the <paramref name="expectations" />.
	/// </summary>
	public DirectoryResult<TFileSystem> WithDirectories(
		Action<IThat<IEnumerable<IDirectoryInfo>>> expectations)
	{
		_expectationBuilder
			.ForMember(
				MemberAccessor<TFileSystem, IEnumerable<IDirectoryInfo>>.FromFunc(
					f => f.Directory.EnumerateDirectories(path).Select(p => f.DirectoryInfo.New(p)), "subdirectories "),
				(property, expectation) => $" which {property}should {expectation}")
			.AddExpectations(e
				=> expectations(new ThatSubject<IEnumerable<IDirectoryInfo>>(e)));
		return this;
	}
}
