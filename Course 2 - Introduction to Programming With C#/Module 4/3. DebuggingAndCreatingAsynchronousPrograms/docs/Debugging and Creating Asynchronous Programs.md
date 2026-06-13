# Debugging and Creating Asynchronous Programs

**Course 2 — Introduction to Programming With C#** · Module 4 · Lesson 3 · `You Try It!`

> Build a small C# console app that runs an **asynchronous** method. You'll use
> `async`/`await` with `Task.Delay` to simulate a long operation, call it from `Main`
> so the program **waits for completion**, trace the flow with `Console.WriteLine`
> "breakpoints," and wrap the work in a `try`/`catch` for robustness.

---

## 🎯 Objective

Understand the `async` and `await` keywords and how they keep programs responsive by
letting tasks run asynchronously. You'll extend the Visual Studio Code console
application you created at the start of the course so it runs an asynchronous method,
observe its execution flow, and add error handling.

---

## 🗂️ What you'll build

A single-file console program built entirely inside **`Program.cs`** (clear out any
existing code first):

| Piece                          | Responsibility                                                    |
| ------------------------------ | ----------------------------------------------------------------- |
| `PerformLongOperationAsync()`  | Simulate a time-consuming operation with `await Task.Delay(3000)` |
| `Console.WriteLine` statements | Act as "breakpoints" to trace the program's flow                  |
| `try` / `catch` block          | Catch and report any exception raised during the operation        |
| `Main(string[] args)`          | Start the async method with `Task.Run` and wait for it to finish  |

**Flow:** `Main → Task.Run(PerformLongOperationAsync) → await Task.Delay(3000) → "completed" → Main continues`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- The console application you created at the start of the course (open its folder in VS Code)

---

## 🛠️ Steps

### Step 1 — Introduction to async and await

The `async` keyword marks a method as asynchronous, and `await` suspends that method
until an awaited `Task` completes — **without blocking** the calling thread. This is what
keeps an application responsive while long-running work is in progress.

- Open the **`Program.cs`** file of your existing console application.
- **Remove any existing code** — you'll build every step below in this one file.

### Step 2 — Implement an asynchronous method

Create a simple asynchronous method that simulates a time-consuming operation using
`Task.Delay`.

- In `Program.cs`, create a class called **`Program`**.
- Inside it, create a method called **`PerformLongOperationAsync`**.
- Use `await Task.Delay` to simulate a delay within the method.

```csharp
class Program
{
    // Step 2: Asynchronous method simulating a long operation
    static async Task PerformLongOperationAsync()
    {
        Console.WriteLine("Operation started...");
        await Task.Delay(3000); // Simulate delay (3 seconds)
        Console.WriteLine("Operation completed after delay.");
    }
}
```

### Step 3 — Call the async method in `Main`

Call the asynchronous method from `Main`, ensuring the program waits for its completion
before exiting.

- Below `PerformLongOperationAsync`, create a **`Main`** method.
- In `Main`, call `PerformLongOperationAsync` using **`Task.Run`**.
- Ensure the program waits for the async method to complete with `task.Wait()`.

```csharp
    // Step 3: Main method - starting point of the app
    static void Main(string[] args)
    {
        // Run the async method and wait for it to complete
        Task task = Task.Run(() => PerformLongOperationAsync());
        task.Wait(); // Ensures the Main thread waits
        Console.WriteLine("Main method completed.");
    }
```

> To check your answer, run the console application. If you receive an error, go to the
> reading on the next page to compare your code to the correct answer.

```bash
dotnet run
```

### Step 4 — Simulate debugging with console statements

Use `Console.WriteLine` statements to simulate breakpoints and observe the flow of the
program.

- Add a `Console.WriteLine` statement **after the `await`** in `PerformLongOperationAsync`
  (the `"Operation completed after delay."` line above already does this).
- Use these statements to understand the program's flow — each line prints in the exact
  order execution reaches it, so you can see when control resumes after the `await`.

### Step 5 — Handle potential errors

Add error handling to the async method to make the code more robust.

- Modify `PerformLongOperationAsync` to include a **`try`/`catch`** block.
- Catch any exceptions that might occur during execution and print an error message.

```csharp
class Program
{
    // Step 2: Asynchronous method simulating a long operation
    static async Task PerformLongOperationAsync()
    {
        try
        {
            Console.WriteLine("Operation started...");
            await Task.Delay(3000); // Simulate delay (3 seconds)
            Console.WriteLine("Operation completed after delay.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    // Step 3: Main method - starting point of the app
    static void Main(string[] args)
    {
        // Run the async method and wait for it to complete
        Task task = Task.Run(() => PerformLongOperationAsync());
        task.Wait(); // Ensures the Main thread waits
        Console.WriteLine("Main method completed.");
    }
}
```

> To check your answer, run the console application. If you receive an error, go to the
> reading on the next page to compare your code to the correct answer.

---

## ▶️ Expected result

Running the app prints the operation's progress in flow order, then confirms `Main`
finished after waiting:

```text
Operation started...
Operation completed after delay.
Main method completed.
```

The ~3-second gap between the first two lines is `await Task.Delay(3000)` at work, and the
final line proves `task.Wait()` held `Main` until the async method completed.

---

## ☑️ Definition of done

- [ ] All existing code removed from `Program.cs`; the new code lives in that one file
- [ ] `PerformLongOperationAsync` is declared `async` and uses `await Task.Delay(3000)`
- [ ] `Main` starts the method with `Task.Run` and waits via `task.Wait()`
- [ ] `Console.WriteLine` statements trace the flow before and after the `await`
- [ ] A `try`/`catch` block reports any exception via `ex.Message`
- [ ] `dotnet run` prints the started / completed / `Main method completed.` lines

---

## 🔑 Key concepts

- **`async` + `await`** — `async` marks a method as asynchronous and `await` suspends it
  until the awaited `Task` finishes, freeing the calling thread to stay responsive.
- **`Task.Delay` vs `Thread.Sleep`** — `await Task.Delay(3000)` simulates work *without*
  blocking a thread, unlike `Thread.Sleep`, which freezes it.
- **Bridging sync and async** — a synchronous `Main` can launch async work with
  `Task.Run(...)` and block on it with `task.Wait()` so the program doesn't exit early.
- **Console statements as breakpoints** — ordered `Console.WriteLine` output makes the
  asynchronous flow visible, showing exactly when execution resumes after an `await`.
- **Robust async code** — wrapping awaited work in `try`/`catch` lets you handle and report
  exceptions instead of crashing the program.
