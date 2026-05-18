using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class FileInfo
		{
			public sealed class AppendTextTests
			{
				[Fact]
				public async Task AppendText_OnSpecificPath_ShouldOnlyCountThatInstance()
				{
					MockFileSystem fileSystem = new();
					fileSystem.FileInfo.New("foo.txt").AppendText().Dispose();
					fileSystem.FileInfo.New("bar.txt").AppendText().Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].AppendText().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WrongPath_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					fileSystem.FileInfo.New("foo.txt").AppendText().Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["bar.txt"].AppendText().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to FileInfo["bar.txt"].AppendText,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class CopyToTests
			{
				[Fact]
				public async Task WhenCopiedTwice_ShouldRecordTwice()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					fileInfo.CopyTo("a.txt");
					fileInfo.CopyTo("b.txt", true);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].CopyTo().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithOverwriteFilter_ShouldOnlyMatchTwoArgOverload()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					fileInfo.CopyTo("a.txt");
					fileInfo.CopyTo("b.txt", true);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].CopyTo(overwrite: o => o).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class CreateAsSymbolicLinkTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "x");
					IFileInfo link = fileSystem.FileInfo.New("link.txt");
					link.CreateAsSymbolicLink("target.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["link.txt"].CreateAsSymbolicLink().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class ExistsTests
			{
				[Fact]
				public async Task ExistsProperty_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").Exists;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Exists.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class IsReadOnlyTests
			{
				[Fact]
				public async Task IsReadOnly_Get_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					_ = fileInfo.IsReadOnly;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].IsReadOnly.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsReadOnly_Set_ShouldFailWhenOnlyRead()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					_ = fileInfo.IsReadOnly;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].IsReadOnly.Set().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once set of FileInfo["foo.txt"].IsReadOnly,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task IsReadOnly_Set_ShouldRecordSetOnce()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					fileInfo.IsReadOnly = false;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].IsReadOnly.Set().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LengthTests
			{
				[Fact]
				public async Task Length_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					_ = fileSystem.FileInfo.New("foo.txt").Length;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Length.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OpenTests
			{
				[Fact]
				public async Task WithModeFilter_ShouldOnlyMatchOverloadsTakingFileMode()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					using (fileInfo.Open(FileMode.Open))
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Open(m => m == FileMode.Open).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OpenReadTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					using (fileInfo.OpenRead())
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].OpenRead().Once();
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
					fileSystem.File.WriteAllText("foo.txt", "");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					fileInfo.Refresh();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Refresh().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReplaceTests
			{
				[Fact]
				public async Task WithIgnoreMetadataErrorsFilter_ShouldOnlyMatchThreeArgOverload()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					fileSystem.File.WriteAllText("dest.txt", "y");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
					fileInfo.Replace("dest.txt", "backup.txt", true);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Replace(ignoreMetadataErrors: i => i).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CreateTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.FileInfo.New("foo.txt").Create().Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Create().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CreateTextTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.FileInfo.New("foo.txt").CreateText().Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].CreateText().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DecryptTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					try
					{
#pragma warning disable CA1416
						fileSystem.FileInfo.New("foo.txt").Decrypt();
#pragma warning restore CA1416
					}
					catch (PlatformNotSupportedException)
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Decrypt().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EncryptTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					try
					{
#pragma warning disable CA1416
						fileSystem.FileInfo.New("foo.txt").Encrypt();
#pragma warning restore CA1416
					}
					catch (PlatformNotSupportedException)
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Encrypt().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DeleteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					fileSystem.FileInfo.New("foo.txt").Delete();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Delete().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class MoveToTests
			{
				[Fact]
				public async Task WithDestFileNameFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					fileSystem.FileInfo.New("foo.txt").MoveTo("bar.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].MoveTo(d => d == "bar.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OpenTextTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");
					fileSystem.FileInfo.New("foo.txt").OpenText().Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].OpenText().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OpenWriteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					fileSystem.FileInfo.New("foo.txt").OpenWrite().Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].OpenWrite().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class ResolveLinkTargetTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "x");
					IFileInfo link = fileSystem.FileInfo.New("link.txt");
					link.CreateAsSymbolicLink("target.txt");
					_ = link.ResolveLinkTarget(false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["link.txt"].ResolveLinkTarget().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class FileSystemInfoPropertyTests
			{
				[Fact]
				public async Task Attributes_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").Attributes;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Attributes.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task CreationTime_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").CreationTime;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].CreationTime.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task CreationTimeUtc_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").CreationTimeUtc;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].CreationTimeUtc.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Directory_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").Directory;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Directory.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DirectoryName_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").DirectoryName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].DirectoryName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Extension_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").Extension;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Extension.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FullName_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").FullName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].FullName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastAccessTime_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").LastAccessTime;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].LastAccessTime.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastAccessTimeUtc_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").LastAccessTimeUtc;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].LastAccessTimeUtc.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastWriteTime_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").LastWriteTime;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].LastWriteTime.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LastWriteTimeUtc_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").LastWriteTimeUtc;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].LastWriteTimeUtc.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task LinkTarget_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.FileInfo.New("foo.txt").LinkTarget;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].LinkTarget.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task UnixFileMode_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					_ = fileSystem.FileInfo.New("foo.txt").UnixFileMode;
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].UnixFileMode.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
#endif
			}

			public sealed class FailureMessageTests
			{
				[Fact]
				public async Task Attributes_Get_WhenNotAccessed_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Attributes.Get().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once get of FileInfo["foo.txt"].Attributes,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Create_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Create().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to FileInfo["foo.txt"].Create,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task MoveTo_WithFilter_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].MoveTo(d => d == "bar.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to FileInfo["foo.txt"].MoveTo with destFileName matching d => d == "bar.txt",
						             but it was recorded 0 times
						             """);
				}
			}
		}
	}
}
