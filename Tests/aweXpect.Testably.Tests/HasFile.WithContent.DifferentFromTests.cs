using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasFile
{
	public sealed partial class WithContent
	{
		public class DifferentFrom
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					string path = "foo.txt";
					IFileSystem sut = new MockFileSystem();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().DifferentFrom("bar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					string path = "foo.txt";
					string content = "bar";
					IFileSystem sut = new MockFileSystem();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, content);

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().DifferentFrom(content);

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content different from "bar",
						              but it did match
						              """);
				}
			}
			
			public sealed class WildcardTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					string path = "foo.txt";
					IFileSystem sut = new MockFileSystem();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().DifferentFrom("b?").AsWildcard();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					string path = "foo.txt";
					IFileSystem sut = new MockFileSystem();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "bar");

					async Task Act()
						=> await That(sut).HasFile(path).WithContent().DifferentFrom("ba?").AsWildcard();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content different from "ba?",
						              but it did match
						              """);
				}
			}
		}
	}
}
