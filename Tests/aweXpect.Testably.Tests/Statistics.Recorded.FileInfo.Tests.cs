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
					fileInfo.CopyTo("b.txt", overwrite: true);

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
					fileInfo.CopyTo("b.txt", overwrite: true);

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
					using (fileInfo.Open(System.IO.FileMode.Open))
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Open(mode: m => m == System.IO.FileMode.Open).Once();
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
					fileInfo.Replace("dest.txt", "backup.txt", ignoreMetadataErrors: true);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo["foo.txt"].Replace(ignoreMetadataErrors: i => i).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
