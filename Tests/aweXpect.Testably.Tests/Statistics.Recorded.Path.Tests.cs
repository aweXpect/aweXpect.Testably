using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class Path
		{
			public sealed class AltDirectorySeparatorCharTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.AltDirectorySeparatorChar;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.AltDirectorySeparatorChar.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

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

			public sealed class DirectorySeparatorCharTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.DirectorySeparatorChar;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.DirectorySeparatorChar.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class EndsInDirectorySeparatorTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.EndsInDirectorySeparator("dir/");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.EndsInDirectorySeparator(p => p == "dir/").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ExistsTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Exists("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.Exists().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

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

#if NET8_0_OR_GREATER
			public sealed class GetRelativePathTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetRelativePath("a", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetRelativePath(r => r == "a", p => p == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

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

#if NET8_0_OR_GREATER
			public sealed class IsPathFullyQualifiedTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.IsPathFullyQualified("/abs");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.IsPathFullyQualified().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class JoinTests
			{
				[Fact]
				public async Task WhenJoinedTwoArgs_ShouldMatch()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Join("a", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(path1: p => p == "a", path2: p => p == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FilterOnPath4_ShouldOnlyMatchFourArgOverload()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Join("a", "b");
					_ = fileSystem.Path.Join("a", "b", "c", "d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(path4: p => p == "d").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class PathSeparatorTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.PathSeparator;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.PathSeparator.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class TrimEndingDirectorySeparatorTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.TrimEndingDirectorySeparator("dir/");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.TrimEndingDirectorySeparator(p => p == "dir/").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class VolumeSeparatorCharTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.VolumeSeparatorChar;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.VolumeSeparatorChar.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
