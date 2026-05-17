using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasName("bar.txt");
				}

				await That(Act).ThrowsException()
					.WithMessage("*has name equal to \"bar.txt\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenNameMatches_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasName(change.Name);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
