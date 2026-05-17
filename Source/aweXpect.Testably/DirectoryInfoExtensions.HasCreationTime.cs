using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the creation time of the <see cref="IDirectoryInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.CreationTime" /> or <see cref="IFileSystemInfo.CreationTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public static TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo>> HasCreationTime(
		this IThat<IDirectoryInfo> source, DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasInfoTimeConstraint<IDirectoryInfo>(it, grammars,
					d => d.CreationTime, tolerance, expected, "creation time")),
			source, tolerance);
	}
}
