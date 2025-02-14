using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public partial class HasFile
{
	public class WhoseContentTests
	{
		[Fact]
		public async Task WhenContentIsDifferent_ShouldFail()
		{
			string path = "foo.txt";
			IFileSystem sut = new MockFileSystem();
			// ReSharper disable once MethodHasAsyncOverload
			sut.File.WriteAllText(path, "baz");

			async Task Act()
				=> await That(sut).HasFile(path).WhoseContent(c => c.IsEmpty());

			await That(Act).ThrowsException()
				.WithMessage($"""
				              Expected that sut
				              has file '{path}' whose content is empty,
				              but content was "baz"
				              """);
		}

		[Fact]
		public async Task WhenContentMatches_ShouldSucceed()
		{
			string path = "foo.txt";
			IFileSystem sut = new MockFileSystem();
			// ReSharper disable once MethodHasAsyncOverload
			sut.File.WriteAllText(path, "");

			async Task Act()
				=> await That(sut).HasFile(path).WhoseContent(c => c.IsEmpty());

			await That(Act).DoesNotThrow();
		}
	}
}
