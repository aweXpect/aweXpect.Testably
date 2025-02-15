# Testably

Expectations for the file and time system from Testably.Abstractions.

## File system

You can verify that a specific file or directory exists in the file system:

```csharp
IFileSystem fileSystem = new MockFileSystem();
//...
await That(fileSystem).HasDirectory("my/path");
await That(fileSystem).HasFile("my-file.txt");
```

### File

For files, you can verify the file content:

```csharp
await That(fileSystem).HasFile("my-file.txt").WithContent("file-content").IgnoringCase();
```

For files, you can verify the creation time, last access time and last write time:

```csharp
await That(sut).HasFile(path).WithCreationTime(expectedTime).Within(1.Second());
await That(sut).HasFile(path).WithLastAccessTime(expectedTime).Within(1.Second());
await That(sut).HasFile(path).LastWriteTime(expectedTime).Within(1.Second());
```

### Directory

For directories, you can verify that they contain subdirectories:

```csharp
await That(sut).HasDirectory(path).WithDirectories(f => f.HasCount().EqualTo(2));
```

For directories, you can verify that they contain files:

```csharp
await That(sut).HasDirectory(path).WithFiles(f => f.All().ComplyWith(x => x.HasContent("SOME-CONTENT")));
```
