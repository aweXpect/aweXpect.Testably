# aweXpect.Testably

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)
[![Build](https://github.com/aweXpect/aweXpect.Testably/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.Testably/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Testably&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Testably)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Testably&metric=coverage)](https://sonarcloud.io/summary/overall?id=aweXpect_aweXpect.Testably)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FaweXpect%2FaweXpect.Testably%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/aweXpect/aweXpect.Testably/main)

Expectations for the file and time system
from [Testably.Abstractions](https://github.com/Testably/Testably.Abstractions).

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
