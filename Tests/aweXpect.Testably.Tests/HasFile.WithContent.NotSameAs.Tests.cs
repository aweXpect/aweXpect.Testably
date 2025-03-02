using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasFile
{
	public sealed partial class WithContent
	{
		public class NotSameAs
		{
			public sealed class StringTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");
					sut.File.WriteAllText(expectedPath, "bar");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string content = "bar";
					string expectedPath = "bar.txt";
					string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, content);
					sut.File.WriteAllText(expectedPath, content);

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath);

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with not the same content as '{fullExpectedPath}',
						              but it did match

						              File content:
						              bar
						              """);
				}
			}

			public sealed class AsWildcardTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");
					sut.File.WriteAllText(expectedPath, "b?");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath).AsWildcard();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					IFileSystem sut = new MockFileSystem();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "bar");
					sut.File.WriteAllText(expectedPath, "ba?");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath).AsWildcard();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with not the same content as '{fullExpectedPath}',
						              but it did match

						              File content:
						              bar
						              """);
				}
			}
		}
	}
}
