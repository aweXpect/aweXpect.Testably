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
		}
	}
}
