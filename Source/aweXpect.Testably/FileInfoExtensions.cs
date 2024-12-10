using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="IFileInfo" />.
/// </summary>
public static partial class FileInfoExtensions
{
	/// <summary>
	///     Start expectations for the <see cref="IFileInfo" /> <paramref name="subject" />.
	/// </summary>
	public static IThat<IFileInfo> Should(this IExpectSubject<IFileInfo> subject)
		=> subject.Should(_ => { });
}
