# Creating Practical Asynchronous Solutions

**Course 2 — Introduction to Programming With C#** · Module 4 · Lesson 2 · `You Try It!`

> Complete two C# exercises where the `async`/`await` keywords have been removed. Fill in the
> blanks so the code runs correctly: **Problem 1** downloads two files concurrently, and
> **Problem 2** processes a dataset in chunks concurrently. A single console app wires both
> together and proves they run end-to-end.

---

## 🎯 Objective

Practice reading and completing asynchronous C# code so you can confidently apply `async` and
`await`: mark methods that return `Task`/`Task<T>` as **`async`**, place **`await`** before any
operation you must wait on, and use `Task.WhenAll` to run independent work **concurrently**.

---

## 🗂️ What you'll build

A single console project named **`CreatingPracticalAsyncSol`** containing two worked problems
driven from `Program.cs`:

| File          | Responsibility                                                          |
| ------------- | ----------------------------------------------------------------------- |
| `Problem1.cs` | Download two files concurrently with `Task.WhenAll`                      |
| `Problem2.cs` | Process N data chunks concurrently with a `List<Task>` + `Task.WhenAll` |
| `Program.cs`  | Run Problem 1, then Problem 2, and report progress                      |

**Flow:** `Program  →  Problem1.DownloadFilesAsync()  →  Problem2.ProcessLargeDatasetAsync(5)  →  done`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (or any C# editor)
- Familiarity with `Task`, `async`, and `await`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold the console app and move into it.

```bash
dotnet new console -n CreatingPracticalAsyncSol
cd CreatingPracticalAsyncSol
```

> The default template targets a current .NET version and already enables top-level statements;
> we replace `Program.cs` with an explicit `Main` so the two problems are easy to follow.

### Step 2 — Problem 1: download files asynchronously

**Problem.** Some async keywords were removed from the code (the `_____` blanks). Fill them in so
the method correctly downloads two files **concurrently** and prints start/completion messages.

The rules to apply:

- Methods that return `Task` or `Task<T>` are marked with **`async`**.
- Any operation you must wait on (here, `Task.Delay` and `Task.WhenAll`) is preceded by **`await`**.

Create **`Problem1.cs`** with the blanks filled (the two `async` keywords and the two `await`
keywords restored):

```csharp
namespace CreatingPracticalAsyncSol;

public class Problem1
{
    public async Task<string> DownloadFileAsync(string fileName)
    {
        Console.WriteLine($"Starting download of {fileName}...");
        await Task.Delay(3000); // Simulate a 3-second download time
        Console.WriteLine($"Completed download of {fileName}.");
        return $"{fileName} content";
    }

    public async Task DownloadFilesAsync()
    {
        // Start downloading "File1.txt" and "File2.txt" concurrently
        var downloadTask1 = DownloadFileAsync("File1.txt");
        var downloadTask2 = DownloadFileAsync("File2.txt");

        // Wait for both downloads to complete
        await Task.WhenAll(downloadTask1, downloadTask2);

        Console.WriteLine("All downloads completed.");
    }
}
```

> The two downloads are **started before either is awaited**, so they overlap. Awaiting each call
> in sequence instead would make total time ~6 s; with `Task.WhenAll` it is ~3 s.

### Step 3 — Problem 2: process data chunks asynchronously

**Problem.** Again the async keywords were removed. Fill them in so the code processes chunks of
data **concurrently** and prints a message when each chunk starts and completes.

Create **`Problem2.cs`** with the blanks filled:

```csharp
namespace CreatingPracticalAsyncSol
{
    public class Problem2
    {
        public async Task ProcessDataChunkAsync(int chunkNumber)
        {
            Console.WriteLine($"Processing chunk {chunkNumber}...");
            await Task.Delay(1000); // Simulate processing time
            Console.WriteLine($"Completed processing of chunk {chunkNumber}.");
        }

        public async Task ProcessLargeDatasetAsync(int numberOfChunks)
        {
            var tasks = new List<Task>();

            // Start processing each chunk concurrently
            for (int i = 1; i <= numberOfChunks; i++)
            {
                tasks.Add(ProcessDataChunkAsync(i));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            Console.WriteLine("All data chunks processed.");
        }
    }
}
```

> The loop **collects tasks without awaiting them**, so all chunks are in flight at once. Awaiting
> inside the loop would serialize them — collect first, then `await Task.WhenAll(tasks)`.

### Step 4 — Wire both problems in `Program.cs`

Replace the contents of `Program.cs` with an async `Main` that runs each problem in turn. `Main`
returns `Task`, so it can use `await` directly.

```csharp
using CreatingPracticalAsyncSol;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Running Program 1 ===");
        var p1 = new Problem1();
        await p1.DownloadFilesAsync();

        Console.WriteLine("\n=== Running Program 2 ===");
        var p2 = new Problem2();
        await p2.ProcessLargeDatasetAsync(5); // Example with 5 chunks

        Console.WriteLine("\n=== All Programs Finished ===");
    }
}
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The two file downloads start almost together and both finish ~3 s later (not ~6 s), then the five
chunks all start, then all complete ~1 s later. Output looks like:

```text
=== Running Program 1 ===
Starting download of File1.txt...
Starting download of File2.txt...
Completed download of File1.txt.
Completed download of File2.txt.
All downloads completed.

=== Running Program 2 ===
Processing chunk 1...
Processing chunk 2...
Processing chunk 3...
Processing chunk 4...
Processing chunk 5...
Completed processing of chunk 1.
Completed processing of chunk 2.
Completed processing of chunk 3.
Completed processing of chunk 4.
Completed processing of chunk 5.
All data chunks processed.

=== All Programs Finished ===
```

> The exact ordering of the "Completed" lines may vary run to run — that is expected for concurrent
> work and confirms the tasks really do overlap.

---

## ☑️ Definition of done

- [ ] `CreatingPracticalAsyncSol` console project created with `dotnet new console`
- [ ] `Problem1.cs` filled in: `DownloadFileAsync`/`DownloadFilesAsync` marked `async`, `Task.Delay` and `Task.WhenAll` awaited
- [ ] `Problem2.cs` filled in: `ProcessDataChunkAsync`/`ProcessLargeDatasetAsync` marked `async`, both awaits restored
- [ ] `Program.cs` has an `async Task Main` that runs Problem 1 then Problem 2
- [ ] `dotnet run` prints start/completion messages and downloads/chunks run concurrently (not sequentially)

---

## 🔑 Key concepts

- **`async` enables `await`** — a method must be marked `async` (and return `Task`, `Task<T>`, or
  `void` for handlers) before you can `await` inside it.
- **`await` yields, it doesn't block** — it suspends the method until the awaited operation
  finishes and frees the calling thread, unlike a blocking `.Wait()` or `.Result`.
- **Start-then-await for concurrency** — capturing tasks first and awaiting `Task.WhenAll` later
  runs work in parallel; awaiting each call inline runs them one after another.
- **`Task.WhenAll` joins many tasks** — it completes only when *every* task does, making it the go-to
  for fan-out/fan-in patterns like batch downloads or chunked processing.
- **`async Task Main`** — entry points can be asynchronous in modern C#, so top-level `await` works
  without `.GetAwaiter().GetResult()` plumbing.
