using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class File
		{
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
		}
	}
}
