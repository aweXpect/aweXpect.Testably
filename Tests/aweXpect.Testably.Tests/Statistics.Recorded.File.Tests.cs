using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class File
		{
			public sealed class AppendAllBytesTests
			{
				[Fact]
				public async Task WhenNeverCalled_ShouldSucceedWithNever()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllBytes().Never();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AppendAllBytesAsyncTests
			{
				[Fact]
				public async Task WhenNeverCalled_ShouldSucceedWithNever()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllBytesAsync().Never();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class AppendAllLinesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.AppendAllLinesAsync("foo.txt", new[] { "line" });

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.AppendAllLinesAsync("foo.txt", new[] { "line" });
					await fileSystem.File.AppendAllLinesAsync("bar.txt", new[] { "line" });

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLinesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AppendAllTextAsyncTests
			{
				[Fact]
				public async Task WhenCalledTwice_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.AppendAllTextAsync("a.txt", "x");
					await fileSystem.File.AppendAllTextAsync("b.txt", "y");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllTextAsync().Twice();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class CopyTests
			{
				[Fact]
				public async Task WhenCopiedTwice_ShouldSucceedWithTwice()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.Copy("a.txt", "b.txt");
					fileSystem.File.Copy("a.txt", "c.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Copy().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithDestFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.Copy("a.txt", "b.txt");
					fileSystem.File.Copy("a.txt", "c.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Copy(destFileName: d => d == "b.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class CreateSymbolicLinkTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "hello");
					fileSystem.File.CreateSymbolicLink("link.txt", "target.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.CreateSymbolicLink().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathToTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.WriteAllText("b.txt", "y");
					fileSystem.File.CreateSymbolicLink("link-a.txt", "a.txt");
					fileSystem.File.CreateSymbolicLink("link-b.txt", "b.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.CreateSymbolicLink(pathToTarget: t => t == "a.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class DeleteTests
			{
				[Fact]
				public async Task CalledOnce_ShouldFailExpectingNever()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					fileSystem.File.Delete("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Delete().Never();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded no call to File.Delete,
						             but it was recorded 1 time
						             """);
				}

				[Fact]
				public async Task NeverCalled_ShouldSucceedWithNever()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Delete().Never();
					}

					await That(Act).DoesNotThrow();
				}
			}
			
			public sealed class ExistsTests
			{
				[Fact]
				public async Task WhenExistsCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.File.Exists("foo.txt");
					_ = fileSystem.File.Exists("bar.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Exists().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.File.Exists("foo.txt");
					_ = fileSystem.File.Exists("bar.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Exists(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
			
#if NET8_0_OR_GREATER
			public sealed class GetUnixFileModeTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					_ = fileSystem.File.GetUnixFileMode("foo.txt");
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.GetUnixFileMode().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAllBytesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await fileSystem.File.ReadAllBytesAsync("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadAllBytesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAllLinesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await fileSystem.File.ReadAllLinesAsync("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadAllLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAllTextAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await fileSystem.File.ReadAllTextAsync("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadAllTextAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadLinesAsyncTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await foreach (string _ in fileSystem.File.ReadLinesAsync("foo.txt"))
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ResolveLinkTargetTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "hello");
					fileSystem.File.CreateSymbolicLink("link.txt", "target.txt");
					_ = fileSystem.File.ResolveLinkTarget("link.txt", returnFinalTarget: false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ResolveLinkTarget().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithReturnFinalTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "hello");
					fileSystem.File.CreateSymbolicLink("link.txt", "target.txt");
					_ = fileSystem.File.ResolveLinkTarget("link.txt", returnFinalTarget: true);
					_ = fileSystem.File.ResolveLinkTarget("link.txt", returnFinalTarget: false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ResolveLinkTarget(returnFinalTarget: r => r).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SetUnixFileModeTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					fileSystem.File.SetUnixFileMode("foo.txt", System.IO.UnixFileMode.UserRead);
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.SetUnixFileMode().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithModeFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					fileSystem.File.SetUnixFileMode("foo.txt", System.IO.UnixFileMode.UserRead);
					fileSystem.File.SetUnixFileMode("foo.txt", System.IO.UnixFileMode.UserWrite);
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.SetUnixFileMode(mode: m => m == System.IO.UnixFileMode.UserRead).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteAllBytesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllBytesAsync("foo.txt", new byte[] { 1, 2, 3 });

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllBytesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteAllLinesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllLinesAsync("foo.txt", new[] { "line" });

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class WriteAllTextTests
			{
				[Fact]
				public async Task FilteringOnEncoding_ShouldOnlyMatchThreeArgOverload()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");
					fileSystem.File.WriteAllText("bar.txt", "world", Encoding.UTF8);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(encoding: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCalledTwice_ShouldSucceedExpectingTwice()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.WriteAllText("b.txt", "y");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllText().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotCalled_ShouldFailExpectingOnce()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to File.WriteAllText,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WithMatchingPathPredicate_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");
					fileSystem.File.WriteAllText("bar.txt", "world");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilterAndNoMatch_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(p => p == "missing.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to File.WriteAllText with path matching p => p == "missing.txt",
						             but it was recorded 0 times
						             """);
				}
			}

#if NET8_0_OR_GREATER
			public sealed class WriteAllTextAsyncTests
			{
				[Fact]
				public async Task WhenCalledTwice_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllTextAsync("a.txt", "x");
					await fileSystem.File.WriteAllTextAsync("b.txt", "y");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllTextAsync().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithMatchingPathPredicate_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllTextAsync("foo.txt", "hello");
					await fileSystem.File.WriteAllTextAsync("bar.txt", "world");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllTextAsync(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif
		}
	}
}
