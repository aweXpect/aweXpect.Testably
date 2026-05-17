using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileVersionInfo
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task GetVersionInfo_WithFileNameFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					fileSystem.File.WriteAllText("b.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("b.exe");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo.GetVersionInfo(fileName: f => f == "a.exe").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FileNameTests
			{
				[Fact]
				public async Task FileName_Get_ShouldRecordOnInstance()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
