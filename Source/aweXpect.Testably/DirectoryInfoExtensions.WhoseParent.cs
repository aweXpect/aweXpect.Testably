#if NET10_0_OR_GREATER
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	extension(IThat<IDirectoryInfo> source)
	{
		/// <summary>
		///     Further expectations on the parent directory of the <see cref="IDirectoryInfo" />.
		/// </summary>
		public IThat<IDirectoryInfo> WhoseParent
			=> new ThatSubject<IDirectoryInfo>(
				source.Get().ExpectationBuilder
					.ForWhich<IDirectoryInfo, IDirectoryInfo>(d => d.Parent!, " whose parent "));
	}
}
#endif
