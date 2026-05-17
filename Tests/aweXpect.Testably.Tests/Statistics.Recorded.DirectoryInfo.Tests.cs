using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class DirectoryInfo
		{
			public sealed class CreateTests
			{
				[Fact]
				public async Task CreateOnInstance_ShouldRecordUnderPathBucket()
				{
					MockFileSystem fileSystem = new();
					fileSystem.DirectoryInfo.New("foo").Create();
					fileSystem.DirectoryInfo.New("bar").Create();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Create().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WrongPath_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					fileSystem.DirectoryInfo.New("foo").Create();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["bar"].Create().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to DirectoryInfo["bar"].Create,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class CreateSubdirectoryTests
			{
				[Fact]
				public async Task WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					di.CreateSubdirectory("alpha");
					di.CreateSubdirectory("beta");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"].CreateSubdirectory(p => p == "alpha").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DeleteTests
			{
				[Fact]
				public async Task WithRecursiveFilter_ShouldOnlyMatchOneArgOverload()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					IDirectoryInfo di2 = fileSystem.DirectoryInfo.New("root2");
					di2.Create();
					di.Delete();
					di2.Delete(recursive: true);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root2"].Delete(recursive: r => r).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerateDirectoriesTests
			{
				[Fact]
				public async Task WithSearchOptionFilter_ShouldOnlyMatchTwoArgOverload()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.EnumerateDirectories().ToList();
					_ = di.EnumerateDirectories("*", SearchOption.AllDirectories).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"]
							.EnumerateDirectories(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).DoesNotThrow();
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task WithEnumerationOptionsFilter_ShouldOnlyMatchEnumerationOverload()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.EnumerateDirectories("*", SearchOption.AllDirectories).ToList();
					_ = di.EnumerateDirectories("*", new EnumerationOptions()).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"]
							.EnumerateDirectories(enumerationOptions: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}
#endif
			}

			public sealed class ExistsTests
			{
				[Fact]
				public async Task ExistsProperty_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DirectoryInfo.New("foo").Exists;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Exists.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class MoveToTests
			{
				[Fact]
				public async Task WithDestDirNameFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo a = fileSystem.DirectoryInfo.New("a");
					a.Create();
					a.MoveTo("b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["a"].MoveTo(destDirName: d => d == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ParentTests
			{
				[Fact]
				public async Task Parent_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DirectoryInfo.New("foo").Parent;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Parent.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class RefreshTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("foo");
					di.Create();
					di.Refresh();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Refresh().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
