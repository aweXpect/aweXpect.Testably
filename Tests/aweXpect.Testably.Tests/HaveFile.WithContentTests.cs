﻿using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HaveFile
{
	public class WithContentTests
	{
		[Fact]
		public async Task WhenContentIsDifferent_ShouldFail()
		{
			string path = "foo.txt";
			IFileSystem sut = new MockFileSystem();
			// ReSharper disable once MethodHasAsyncOverload
			sut.File.WriteAllText(path, "baz");

			async Task Act()
				=> await That(sut).Should().HaveFile(path).WithContent("bar");

			await That(Act).Should().ThrowException()
				.WithMessage($"""
				              Expected sut to
				              have file '{path}' with content "bar",
				              but it was "baz" which differs at index 2:
				                   ↓ (actual)
				                "baz"
				                "bar"
				                   ↑ (expected)
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
				=> await That(sut).Should().HaveFile(path).WithContent(content);

			await That(Act).Should().NotThrow();
		}
	}
}
