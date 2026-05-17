using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class FileInfo
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task WhenFactoryCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().FileInfo.New().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
