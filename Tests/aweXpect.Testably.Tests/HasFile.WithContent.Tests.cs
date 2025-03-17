using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasFile
{
	public sealed partial class WithContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenContentIsDifferent_ShouldFail()
			{
				string path = "foo.txt";
				IFileSystem sut = new MockFileSystem();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "baz");

				async Task Act()
					=> await That(sut).HasFile(path).WithContent("bar");

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that sut
					              has file '{path}' with content equal to "bar",
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
				string path = "foo.txt";
				string content = "bar";
				IFileSystem sut = new MockFileSystem();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, content);

				async Task Act()
					=> await That(sut).HasFile(path).WithContent(content);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsWildcardTests
		{
			[Fact]
			public async Task WhenContentIsDifferent_ShouldFail()
			{
				string path = "foo.txt";
				IFileSystem sut = new MockFileSystem();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "baz");

				async Task Act()
					=> await That(sut).HasFile(path).WithContent("b?").AsWildcard();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that sut
					              has file '{path}' with content matching "b?",
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
				string path = "foo.txt";
				IFileSystem sut = new MockFileSystem();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "bar");

				async Task Act()
					=> await That(sut).HasFile(path).WithContent("ba?").AsWildcard();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
