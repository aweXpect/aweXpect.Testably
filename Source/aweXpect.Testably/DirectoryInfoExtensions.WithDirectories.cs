using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the subdirectories of the <see cref="IDirectoryInfo" /> satisfy the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> WithDirectories(
		this IThat<IDirectoryInfo> source,
		Action<IThat<IEnumerable<IDirectoryInfo>>> expectations)
	{
		ExpectationBuilder builder = source.Get().ExpectationBuilder
			.ForMember(
				MemberAccessor<IDirectoryInfo, IEnumerable<IDirectoryInfo>>.FromFunc(
					d => d.FileSystem.Directory.EnumerateDirectories(d.FullName)
						.Select(p => d.FileSystem.DirectoryInfo.New(p)),
					"subdirectories "),
				(member, expectation) => expectation.Append(" whose ").Append(member))
			.AddExpectations(
				e => expectations(new ThatSubject<IEnumerable<IDirectoryInfo>>(e)),
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return new AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>>(builder, source);
	}
}
