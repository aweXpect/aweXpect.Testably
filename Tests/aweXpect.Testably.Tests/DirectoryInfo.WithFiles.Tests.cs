using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class WithFiles
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllFilesMatchPredicate_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).WithFiles(f =>
						f.All().ComplyWith(x => x.HasContent("some-content")));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeFilesDoNotMatch_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).WithFiles(f =>
						f.All().ComplyWith(x => x.HasContent("other")));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that dirInfo
					              whose files have content equal to "other" for all items,
					             but not all were

					             File content:
					             some-content
					             """);
			}
		}
	}
}
