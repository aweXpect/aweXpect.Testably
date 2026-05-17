using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileStream
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task New_WithModeFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None))
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(mode: m => m == FileMode.Open).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteByteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						s.WriteByte(99);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].WriteByte(value: v => v == 99).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LengthTests
			{
				[Fact]
				public async Task Length_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.Length;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Length.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
