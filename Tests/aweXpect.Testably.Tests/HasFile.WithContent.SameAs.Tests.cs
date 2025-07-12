using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class HasFile
{
	public sealed partial class WithContent
	{
		public class SameAs
		{
			public sealed class StringTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");
					sut.File.WriteAllText(expectedPath, "bar");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().SameAs(expectedPath);

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with the same content as '{fullExpectedPath}',
						              but it was "baz" which differs at index 2:
						                   ↓ (actual)
						                "baz"
						                "bar"
						                   ↑ (expected)

						              File content:
						              baz
						              """);
				}

				[Fact]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string content = "bar";
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, content);
					sut.File.WriteAllText(expectedPath, content);

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().SameAs(expectedPath);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AsWildcardTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");
					sut.File.WriteAllText(expectedPath, "b?");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().SameAs(expectedPath).AsWildcard();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with the same content as '{fullExpectedPath}',
						              but it did not match:
						                ↓ (actual)
						                "baz"
						                "b?"
						                ↑ (wildcard pattern)

						              File content:
						              baz
						              """);
				}

				[Fact]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "bar");
					sut.File.WriteAllText(expectedPath, "ba?");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().SameAs(expectedPath).AsWildcard();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
