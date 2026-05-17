using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class DriveInfo
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task GetDrives_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DriveInfo.GetDrives();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().DriveInfo.GetDrives().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task New_WithDriveNameFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DriveInfo.New("C:");
					_ = fileSystem.DriveInfo.New("D:");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo.New(driveName: n => n == "C:").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NameTests
			{
				[Fact]
				public async Task Name_Get_ShouldRecordPerDrive()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DriveInfo.New("C:").Name;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].Name.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
