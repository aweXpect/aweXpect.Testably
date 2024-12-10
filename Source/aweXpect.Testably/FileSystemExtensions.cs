using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="IFileSystem" />.
/// </summary>
public static partial class FileSystemExtensions
{
	/// <summary>
	///     Start expectations for the <see cref="IFileSystem" /> <paramref name="subject" />.
	/// </summary>
	public static IThat<TFileSystem> Should<TFileSystem>(this IExpectSubject<TFileSystem> subject)
		where TFileSystem : IFileSystem
		=> subject.Should(_ => { });
}
