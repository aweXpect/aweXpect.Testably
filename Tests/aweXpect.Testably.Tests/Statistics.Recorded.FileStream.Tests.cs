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
							.FileStream["a.txt"].WriteByte(v => v == 99).Once();
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

			public sealed class CloseTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None);
					s.Close();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Close().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CopyToTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream src = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					using (MemoryStream dest = new())
					{
						src.CopyTo(dest, 4096);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CopyTo(bufferSize: b => b == 4096).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CopyToAsyncTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream src = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					using (MemoryStream dest = new())
					{
						await src.CopyToAsync(dest);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CopyToAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FlushTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						s.Flush();
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Flush().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FlushAsyncTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						await s.FlushAsync();
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].FlushAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					{
						byte[] buffer = new byte[5];
						_ = s.Read(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Read(count: c => c == 5).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAsyncTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					{
						byte[] buffer = new byte[5];
						_ = await s.ReadAsync(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].ReadAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadByteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					{
						_ = s.ReadByte();
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].ReadByte().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SeekTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.ReadWrite))
					{
						s.Seek(1, SeekOrigin.Begin);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Seek(origin: o => o == SeekOrigin.Begin).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SetLengthTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						s.SetLength(10);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].SetLength(v => v == 10).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						byte[] buffer = new byte[]
						{
							1, 2, 3,
						};
						s.Write(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Write(count: c => c == 3).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteAsyncTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						byte[] buffer = new byte[]
						{
							1, 2, 3,
						};
						await s.WriteAsync(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].WriteAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanReadTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanRead;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanRead.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanSeekTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanSeek;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanSeek.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanTimeoutTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanTimeout;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanTimeout.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanWriteTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanWrite;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanWrite.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class IsAsyncTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.IsAsync;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].IsAsync.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NameTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.Name;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Name.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class PositionTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.Position;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Position.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadTimeoutTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						try { _ = s.ReadTimeout; }
						catch (InvalidOperationException) { }
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].ReadTimeout.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteTimeoutTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						try { _ = s.WriteTimeout; }
						catch (InvalidOperationException) { }
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].WriteTimeout.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FailureMessageTests
			{
				[Fact]
				public async Task Close_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Close().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to FileStream["a.txt"].Close,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Length_Get_WhenNotAccessed_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Length.Get().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once get of FileStream["a.txt"].Length,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Read_WithFilter_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Read(count: c => c == 5).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded exactly once call to FileStream["a.txt"].Read with count matching c => c == 5,
						             but it was recorded 0 times
						             """);
				}
			}
		}
	}
}
