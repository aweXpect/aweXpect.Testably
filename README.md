# aweXpect.Testably

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)
[![Build](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Testably)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Testably)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Testably%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Testably/main)

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

### IFileInfo / IDirectoryInfo as subjects

You can also assert directly on `IFileInfo` and `IDirectoryInfo` instances:

```csharp
IFileInfo fileInfo = fileSystem.FileInfo.New("my-file.txt");
await That(fileInfo).HasLength(12).And.HasContent("some content");
await That(fileInfo).HasName("my-file.txt").And.HasExtension(".txt");
await That(fileInfo).Exists();

IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");
await That(dirInfo).IsNotEmpty();
await That(dirInfo).HasFile("bar/my-file.txt");
await That(dirInfo).HasDirectory("bar").Which.HasFile("my-file.txt");
```

### Bridging from the file-system chain via `.Which`

`HasFile` and `HasDirectory` expose a `.Which` property that returns the
`IThat<IFileInfo>` / `IThat<IDirectoryInfo>` for the resolved entry, so the
same assertions light up in both places:

```csharp
await That(fileSystem).HasFile("my-file.txt").Which.HasLength(12).And.HasContent("some content");
await That(fileSystem).HasDirectory("logs").Which.IsEmpty();
```

### IFileVersionInfo

`IFileVersionInfo` instances obtained via `MockFileSystem.FileVersionInfo.GetVersionInfo`
can be asserted directly. The configured values come from
`MockFileSystem.WithFileVersionInfo(glob, builder)`:

```csharp
MockFileSystem fileSystem = new();
fileSystem.WithFileVersionInfo("*.dll", v => v
    .SetCompanyName("Acme")
    .SetProductName("Anvil")
    .SetFileVersion("1.2.3.4")
    .SetIsDebug(true));
fileSystem.File.WriteAllText("Acme.dll", "");

IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

await That(info).HasCompanyName("Acme").And.HasProductName("Anvil");
await That(info).HasFileVersion("1.2.3.4");
await That(info).IsDebug().And.IsNotPreRelease();
```

The following common fields have dedicated assertions:
`HasCompanyName`, `HasProductName`, `HasFileDescription`, `HasFileVersion`,
`HasProductVersion`, `HasOriginalFilename`, `HasLanguage`, plus the bool pairs
`IsDebug` / `IsNotDebug`, `IsPreRelease` / `IsNotPreRelease`,
`IsPatched` / `IsNotPatched`. For the remaining properties (e.g. `Comments`,
`LegalCopyright`, `FileMajorPart`), assert on them directly via
`await That(info.LegalCopyright).IsEqualTo("…")`.

### Notifications

A `MockFileSystem` raises notifications when files or directories change. Run
the code under test, then assert against the notifications it produced:

```csharp
MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("my-file.txt", "some content");

await That(fileSystem).TriggeredNotification();
await That(fileSystem).TriggeredNotification(c => c.Name == "my-file.txt");
```

`.Within(timeout)` (default 30 s) lets the assertion wait for asynchronous
notifications — if a matching notification already fired the assertion
completes synchronously, otherwise it waits up to the timeout for a late
arrival:

```csharp
_ = Task.Run(() => fileSystem.File.WriteAllText("foo.txt", "x"));
await That(fileSystem).TriggeredNotification().Within(100.Milliseconds());
```

`DidNotTriggerNotification` mirrors the same shapes and short-circuits as soon
as a matching notification is observed:

```csharp
await That(fileSystem).DidNotTriggerNotification().Within(100.Milliseconds());
await That(fileSystem).DidNotTriggerNotification(c => c.Name == "secret.txt");
```

`TriggeredNotification` accepts a `Quantifier` (`AtLeast`, `AtMost`, `Exactly`,
`Between`, `Never`, `Once`) so you can assert how often the notification fires:

```csharp
fileSystem.File.WriteAllText("a.txt", "x");
fileSystem.File.WriteAllText("b.txt", "y");

await That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
    .Exactly(2.Times());
```

Both `TriggeredNotification` and `DidNotTriggerNotification` expose
`.Which(c => …)`, which applies inner expectations from `ChangeDescriptionExtensions`
as an additional per-notification filter — only changes that satisfy them count:

```csharp
fileSystem.File.WriteAllText("a.txt", "x");

await That(fileSystem)
    .TriggeredNotification()
    .Which(c => c.HasName("a.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

> Replay of historical notifications relies on the `MockFileSystem` notification
> history. Disable it via
> `new MockFileSystem(o => o.WithoutNotificationHistory())` only if you don't
> use these assertions — they throw against a history-disabled file system.

### Watcher events

An individual `IFileSystemWatcher` can also be the subject. The watcher must
come from a `MockFileSystem`, and `EnableRaisingEvents` must be `true` for any
event to be observed:

```csharp
MockFileSystem fileSystem = new();
using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/");
watcher.EnableRaisingEvents = true;
fileSystem.File.WriteAllText("my-file.txt", "some content");

await That(watcher).Triggered();
await That(watcher).Triggered(c => c.Name == "my-file.txt");
```

Only events that originate from this specific watcher count — events fired on
other watchers of the same `MockFileSystem` are ignored.

`.Within(timeout)` (default 30 s) lets the assertion wait for asynchronous
events:

```csharp
_ = Task.Run(() => fileSystem.File.WriteAllText("foo.txt", "x"));
await That(watcher).Triggered().Within(100.Milliseconds());
```

`DidNotTrigger` mirrors the same shapes and short-circuits as soon as a
matching event is observed:

```csharp
await That(watcher).DidNotTrigger().Within(100.Milliseconds());
await That(watcher).DidNotTrigger(c => c.Name == "secret.txt");
```

Both `Triggered` and `DidNotTrigger` accept either a synchronous
`Func<WatcherChangeDescription, bool>` predicate or a `.Which(c => …)` callback
that composes inner expectations from `ChangeDescriptionExtensions`:

```csharp
await That(watcher)
    .Triggered()
    .Which(c => c.HasName("my-file.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

### `ChangeDescription` as a subject

Individual `ChangeDescription` instances can be asserted directly. The
`HasChangeType`, `HasFileSystemType` and `HasNotifyFilters` assertions use flag
containment (so a `LastWrite | FileName` change satisfies
`HasNotifyFilters(NotifyFilters.LastWrite)`); the empty / `default` value is
rejected with an `ArgumentException` to avoid silent passes.

```csharp
await That(change).HasChangeType(WatcherChangeTypes.Created);
await That(change).DoesNotHaveChangeType(WatcherChangeTypes.Deleted);

await That(change).HasFileSystemType(FileSystemTypes.File);
await That(change).HasNotifyFilters(NotifyFilters.LastWrite);

await That(change).HasName("my-file.txt").And.HasPath("/abs/my-file.txt");
await That(renamedChange).HasOldName("old.txt").And.HasOldPath("/abs/old.txt");
```

## Time system

### Timers

A `MockTimeSystem` exposes timers as `ITimerMock`. You can assert how often the
timer callback was executed without blocking the test thread:

```csharp
MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
    _ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

await That(timer).Executed(3.Times()).Within(5.Seconds());
```

`Executed()` accepts a `Quantifier` (`AtLeast`, `AtMost`, `Exactly`,
`Between`, …) and exposes `.Within(timeout)` for asynchronous execution. The
assertion polls `ITimerMock.ExecutionCount` until the quantifier is satisfied
or the timeout expires — 30 seconds by default.

```csharp
await That(timer).Executed().AtLeast(2.Times()).Within(100.Milliseconds());
```
