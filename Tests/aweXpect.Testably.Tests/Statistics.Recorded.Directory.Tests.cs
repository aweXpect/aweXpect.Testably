using System.IO;
using System.Linq;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class Directory
		{
			public sealed class CreateDirectoryTests
			{
				[Fact]
				public async Task WhenCalled_ShouldMatchOnce()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.CreateDirectory().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_ShouldRejectNonMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateDirectory(p => p == "other").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to Directory.CreateDirectory with path matching p => p == "other",
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class DeleteTests
			{
				[Fact]
				public async Task WithAtLeastOnce_ShouldSucceedAfterTwoCalls()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("a");
					fileSystem.Directory.CreateDirectory("b");
					fileSystem.Directory.Delete("a");
					fileSystem.Directory.Delete("b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.Delete().AtLeast().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerateDirectoriesTests
			{
				[Fact]
				public async Task EnumerateDirectories_FilteringSearchOption_ShouldOnlyMatchThreeArg()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateDirectories("d").ToList();
					_ = fileSystem.Directory.EnumerateDirectories("d", "*", SearchOption.AllDirectories).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerateFilesTests
			{
				[Fact]
				public async Task EnumerateFiles_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateFiles("d").ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFiles().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFilesTests
			{
				[Fact]
				public async Task GetFiles_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetFiles("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFiles(p => p == "d").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
