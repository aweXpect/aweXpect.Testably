using TFsChangeDescription = Testably.Abstractions.Testing.FileSystem.ChangeDescription;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescription
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameDiffers_ShouldFail()
			{
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

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
				TFsChangeDescription change = await CaptureAsync(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasName(change.Name);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
