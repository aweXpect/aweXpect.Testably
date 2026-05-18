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
					di2.Delete(true);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root2"].Delete(r => r).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerateDirectoriesTests
			{
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
							.DirectoryInfo["a"].MoveTo(d => d == "b").Once();
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

			public sealed class EnumerateFilesTests
			{
				[Fact]
				public async Task WithSearchOptionFilter_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.EnumerateFiles("*", SearchOption.AllDirectories).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"]
							.EnumerateFiles(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerateFileSystemInfosTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.EnumerateFileSystemInfos().ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"].EnumerateFileSystemInfos().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetDirectoriesTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.GetDirectories();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"].GetDirectories().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFilesTests
			{
				[Fact]
				public async Task WithSearchPatternFilter_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.GetFiles("*.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"].GetFiles(s => s == "*.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFileSystemInfosTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					IDirectoryInfo di = fileSystem.DirectoryInfo.New("root");
					di.Create();
					_ = di.GetFileSystemInfos();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["root"].GetFileSystemInfos().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FileSystemInfoPropertyTests
			{
				[Fact]
				public async Task Attributes_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").Attributes;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Attributes.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task CreationTime_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").CreationTime;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].CreationTime.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task CreationTimeUtc_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").CreationTimeUtc;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].CreationTimeUtc.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Extension_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").Extension;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Extension.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FullName_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").FullName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].FullName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastAccessTime_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").LastAccessTime;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].LastAccessTime.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastAccessTimeUtc_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").LastAccessTimeUtc;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].LastAccessTimeUtc.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastWriteTime_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").LastWriteTime;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].LastWriteTime.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastWriteTimeUtc_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").LastWriteTimeUtc;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].LastWriteTimeUtc.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Name_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").Name;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Name.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Root_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").Root;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].Root.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task LinkTarget_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");
					_ = fileSystem.DirectoryInfo.New("foo").LinkTarget;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].LinkTarget.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task UnixFileMode_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.Directory.CreateDirectory("foo");
#pragma warning disable CA1416
					_ = fileSystem.DirectoryInfo.New("foo").UnixFileMode;
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["foo"].UnixFileMode.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
#endif
			}

#if NET8_0_OR_GREATER
			public sealed class CreateAsSymbolicLinkTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					IDirectoryInfo link = fileSystem.DirectoryInfo.New("link");
					link.CreateAsSymbolicLink("target");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["link"].CreateAsSymbolicLink(t => t == "target").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ResolveLinkTargetTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					IDirectoryInfo link = fileSystem.DirectoryInfo.New("link");
					link.CreateAsSymbolicLink("target");
					_ = link.ResolveLinkTarget(false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo["link"].ResolveLinkTarget().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif
		}
	}
}
