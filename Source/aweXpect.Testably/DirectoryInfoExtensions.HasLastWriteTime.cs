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
	///     Verifies that the last write time of the <see cref="IDirectoryInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastWriteTime" /> or <see cref="IFileSystemInfo.LastWriteTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public static TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo>> HasLastWriteTime(
		this IThat<IDirectoryInfo> source, DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasInfoTimeConstraint<IDirectoryInfo>(it, grammars,
					d => d.LastWriteTime, tolerance, expected, "last write time")),
			source, tolerance);
	}
}
