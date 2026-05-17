#if NET10_0_OR_GREATER
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	extension(IThat<IFileInfo> source)
	{
		/// <summary>
		///     Further expectations on the parent directory of the <see cref="IFileInfo" />.
		/// </summary>
		public IThat<IDirectoryInfo> WhoseParent
			=> new ThatSubject<IDirectoryInfo>(
				source.Get().ExpectationBuilder
					.ForWhich<IFileInfo, IDirectoryInfo>(f => f.Directory!, " whose parent "));
	}
}
#endif
