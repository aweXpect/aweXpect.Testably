using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileSystemWatcher
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task New_WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("a");
					fileSystem.Directory.CreateDirectory("b");
					_ = fileSystem.FileSystemWatcher.New("a");
					_ = fileSystem.FileSystemWatcher.New("b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher.New(path: p => p == "a").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class BeginInitTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					w.BeginInit();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].BeginInit().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FilterPropertyTests
			{
				[Fact]
				public async Task Filter_Set_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					w.Filter = "*.txt";

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].Filter.Set().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
