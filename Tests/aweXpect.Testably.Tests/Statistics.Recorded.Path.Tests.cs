using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class Path
		{
			public sealed class CombineTests
			{
				[Fact]
				public async Task WhenCombineWithTwoArgs_ShouldMatch()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Combine("a", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(path1: p => p == "a", path2: p => p == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FilterOnPath3_ShouldOnlyMatchThreeArgOverload()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Combine("a", "b");
					_ = fileSystem.Path.Combine("a", "b", "c");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(path3: p => p == "c").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFileNameTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetFileName("dir/foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFileName(p => p == "dir/foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotCalled_ShouldFailWithOnce()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.GetFileName().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to Path.GetFileName,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class GetTempPathTests
			{
				[Fact]
				public async Task NoArgMethod_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetTempPath();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.GetTempPath().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
