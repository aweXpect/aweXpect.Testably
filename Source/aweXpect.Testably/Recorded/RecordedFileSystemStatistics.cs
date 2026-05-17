using aweXpect.Core;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Mirror of <see cref="IFileSystemStatistics" /> for fluent assertions on recorded calls.
/// </summary>
public sealed class RecordedFileSystemStatistics
{
	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileSystemStatistics(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on calls recorded against <see cref="System.IO.Abstractions.IFile" />.
	/// </summary>
	public RecordedFileMethods File => new(_subject);

	/// <summary>
	///     Assertions on calls recorded against <see cref="System.IO.Abstractions.IDirectory" />.
	/// </summary>
	public RecordedDirectoryMethods Directory => new(_subject);

	/// <summary>
	///     Assertions on calls recorded against <see cref="System.IO.Abstractions.IDirectoryInfoFactory" />
	///     and individual <see cref="System.IO.Abstractions.IDirectoryInfo" /> instances.
	/// </summary>
	public RecordedDirectoryInfoBucket DirectoryInfo => new(_subject);

	/// <summary>
	///     Assertions on calls recorded against <see cref="System.IO.Abstractions.IFileInfoFactory" />
	///     and individual <see cref="System.IO.Abstractions.IFileInfo" /> instances.
	/// </summary>
	public RecordedFileInfoBucket FileInfo => new(_subject);

	/// <summary>
	///     Assertions on calls recorded against <see cref="System.IO.Abstractions.IPath" />.
	/// </summary>
	public RecordedPathMethods Path => new(_subject);
}
