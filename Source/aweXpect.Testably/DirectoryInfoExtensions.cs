using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="IDirectoryInfo" />.
/// </summary>
public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Start expectations for the <see cref="IDirectoryInfo" /> <paramref name="subject" />.
	/// </summary>
	public static IThat<IDirectoryInfo> Should(this IExpectSubject<IDirectoryInfo> subject)
		=> subject.Should(_ => { });
}
