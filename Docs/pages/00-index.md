# [aweXpect.Testably](https://github.com/aweXpect/aweXpect.Testably) [![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)

Expectations for the file and time system from [Testably.Abstractions](https://github.com/Testably/Testably.Abstractions).


## File system

You can verify that a specific file or directory exists in the file system:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("my/path");
fileSystem.File.WriteAllText("my-file.txt", "some content");

await That(fileSystem).HasDirectory("my/path");
await That(fileSystem).HasFile("my-file.txt");
```

### File

For files, you can verify the file content:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("my-file.txt", "some content");

await That(fileSystem).HasFile("my-file.txt").WithContent("some content").IgnoringCase();
await That(fileSystem).HasFile("my-file.txt").WithContent().NotEqualTo("some unexpected content");
```

You can also verify the file content with regard to another file:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("my-file.txt", "some content");
fileSystem.File.WriteAllText("my-other-file.txt", "SOME CONTENT");
fileSystem.File.WriteAllText("my-third-file.txt", "some other content");

await That(fileSystem).HasFile("my-file.txt").WithContent().SameAs("my-other-file.txt").IgnoringCase();
await That(fileSystem).HasFile("my-file.txt").WithContent().NotSameAs("my-third-file.txt");
```

For files, you can verify the creation time, last access time and last write time:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("my-file.txt", "some content");

await That(sut).HasFile(path).WithCreationTime(DateTime.Now).Within(1.Second());
await That(sut).HasFile(path).WithLastAccessTime(DateTime.Now).Within(1.Second());
await That(sut).HasFile(path).LastWriteTime(DateTime.Now).Within(1.Second());
```

### Directory

For directories, you can verify that they contain subdirectories:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("foo/bar1");
fileSystem.Directory.CreateDirectory("foo/bar2/baz");

await That(fileSystem).HasDirectory("foo").WithDirectories(f => f.HasCount().EqualTo(2));
```

For directories, you can verify that they contain files:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("foo/bar");
fileSystem.File.WriteAllText("foo/bar/my-file.txt", "some content");

await That(fileSystem).HasDirectory("foo/bar").WithFiles(f => f.All().ComplyWith(x => x.HasContent("SOME CONTENT").IgnoringCase()));
```
