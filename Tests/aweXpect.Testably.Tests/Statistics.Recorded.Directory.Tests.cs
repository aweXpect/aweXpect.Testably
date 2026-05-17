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

#if NET8_0_OR_GREATER
				[Fact]
				public async Task WithUnixCreateModeFilter_ShouldOnlyMatchTwoArgOverload()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.Directory.CreateDirectory("a");
#pragma warning disable CA1416
					fileSystem.Directory.CreateDirectory("b", System.IO.UnixFileMode.UserRead);
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateDirectory(unixCreateMode: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}
#endif
			}

#if NET8_0_OR_GREATER
			public sealed class CreateSymbolicLinkTests
			{
				[Fact]
				public async Task WhenCalled_ShouldMatchOnce()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					fileSystem.Directory.CreateSymbolicLink("link", "target");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.CreateSymbolicLink().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathToTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("a");
					fileSystem.Directory.CreateDirectory("b");
					fileSystem.Directory.CreateSymbolicLink("link-a", "a");
					fileSystem.Directory.CreateSymbolicLink("link-b", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateSymbolicLink(pathToTarget: t => t == "a").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CreateTempSubdirectoryTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Directory.CreateTempSubdirectory("pre-");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.CreateTempSubdirectory().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPrefixFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Directory.CreateTempSubdirectory("alpha-");
					_ = fileSystem.Directory.CreateTempSubdirectory("beta-");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateTempSubdirectory(prefix: p => p == "alpha-").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

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

#if NET8_0_OR_GREATER
				[Fact]
				public async Task FilteringEnumerationOptions_ShouldOnlyMatchEnumerationOverload()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateDirectories("d", "*", SearchOption.AllDirectories).ToList();
					_ = fileSystem.Directory.EnumerateDirectories("d", "*", new EnumerationOptions()).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(enumerationOptions: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}
#endif
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

#if NET8_0_OR_GREATER
			public sealed class ResolveLinkTargetTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					fileSystem.Directory.CreateSymbolicLink("link", "target");
					_ = fileSystem.Directory.ResolveLinkTarget("link", returnFinalTarget: false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.ResolveLinkTarget().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithReturnFinalTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					fileSystem.Directory.CreateSymbolicLink("link", "target");
					_ = fileSystem.Directory.ResolveLinkTarget("link", returnFinalTarget: true);
					_ = fileSystem.Directory.ResolveLinkTarget("link", returnFinalTarget: false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.ResolveLinkTarget(returnFinalTarget: r => r).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif
		}
	}
}
