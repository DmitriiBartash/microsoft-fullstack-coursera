# Create a Small Project and Debug a Program

**Course 2 — Introduction to Programming With C#** · Module 2 · Lesson 3 · `You Try It!`

> Build a small C# console app that manages a simple **to-do list** — add tasks, view
> them, and mark them as completed. Then introduce a common error on purpose and walk
> through **finding and fixing it** with a debug line and graceful range checking.

---

## 🎯 Objective

Practice writing a small, menu-driven console application from scratch in the Visual Studio
Code project you created at the start of the course, then learn a basic **debugging** workflow:
add a diagnostic line, reproduce the problem, and handle the error gracefully so bad input no
longer crashes the program.

---

## 🗂️ What you will build

A single-file console app whose logic lives in one `ToDoList` class. The user is shown a menu
on a loop and chooses an action until they exit.

| Member             | Responsibility                                              |
| ------------------ | ----------------------------------------------------------- |
| `tasks` (array)    | Stores up to **10** task strings                            |
| `taskCount` (int)  | Tracks how many tasks have been added                       |
| `AddTask()`        | Prompts for a task and stores it in `tasks`                 |
| `ViewTasks()`      | Loops through `tasks` and prints each one                   |
| `CompleteTask()`   | Marks a chosen task as `[Completed]` (with range checking)  |
| `DeleteTask()`     | Removes a chosen task and shifts the rest down              |
| `Main()`           | Shows the menu and routes the user's choice on a loop       |

**Flow:** `Main menu  →  choice  →  AddTask / ViewTasks / CompleteTask / DeleteTask  →  loop until Exit`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the C# extension
- The console application you created at the start of the course
- Remove any existing code in `Program.cs` — you will build every step in that file

---

## 🛠️ Steps

### Step 1 — Plan the application

You'll reuse the Visual Studio Code console application you created at the start of the course.
The app lets users **add tasks**, **view** the list, and **mark tasks as completed**. Open
`Program.cs`, remove any existing code, and build each step below in that one file.

If you need a fresh project instead, scaffold one and open it:

```bash
dotnet new console -n ToDoApp
cd ToDoApp
code .
```

### Step 2 — Create the task list

Define a class called `ToDoList`, an array `tasks` that can hold up to **10** tasks, and a
variable `taskCount` to keep track of how many tasks have been added.

```csharp
class ToDoList
{
    static readonly string[] tasks = new string[10];
    static int taskCount = 0;
}
```

### Step 3 — Add a task

Inside the class, write `AddTask`. It prompts the user to enter a task, stores it in the
`tasks` array, and **increases** `taskCount` each time a task is added.

```csharp
static void AddTask()
{
    if (taskCount >= tasks.Length)
    {
        Console.WriteLine("⚠️ Task list is full.");
        return;
    }

    Console.Write("Enter a new task: ");
    string? task = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(task))
    {
        tasks[taskCount++] = task;
        Console.WriteLine("✅ Task added.");
    }
    else
    {
        Console.WriteLine("❌ Error: Task cannot be empty.");
    }
}
```

### Step 4 — View tasks

Below any existing methods, write `ViewTasks`. It loops through the `tasks` array and prints
each task, using `taskCount` to know how many tasks to print.

```csharp
static void ViewTasks()
{
    if (taskCount == 0)
    {
        Console.WriteLine("📭 No tasks to display.");
        return;
    }

    Console.WriteLine("\n📋 To-Do List:");
    for (int i = 0; i < taskCount; i++)
    {
        Console.WriteLine($"{i + 1}. {tasks[i]}");
    }
}
```

### Step 5 — Mark a task as completed

Below any existing methods, write `CompleteTask`. It asks the user to select a task to mark as
complete, then updates the selected task to show that it's done by appending `[Completed]`.

```csharp
static void CompleteTask()
{
    Console.Write("Enter the number of the task to mark as completed: ");
    string? input = Console.ReadLine();

    if (int.TryParse(input, out int taskNumber))
    {
        tasks[taskNumber - 1] += " [Completed]";
        Console.WriteLine("✅ Task marked as completed.");
    }
    else
    {
        Console.WriteLine("❌ Error: Please enter a valid number.");
    }
}
```

> *(You will harden this method against out-of-range input in Step 7.)*

### Step 6 — Run the program

Write the `Main` method that brings everything together. It presents a menu to the user and
uses a loop to keep asking for input until they choose to exit. A `DeleteTask` option is
included so the menu is complete.

```csharp
static void DeleteTask()
{
    Console.Write("Enter the number of the task to delete: ");
    string? input = Console.ReadLine();

    if (int.TryParse(input, out int taskNumber))
    {
        if (taskNumber < 1 || taskNumber > taskCount)
        {
            Console.WriteLine("❌ Error: Invalid task number.");
            return;
        }

        for (int i = taskNumber - 1; i < taskCount - 1; i++)
        {
            tasks[i] = tasks[i + 1];
        }

        tasks[taskCount - 1] = null!;
        taskCount--;
        Console.WriteLine("🗑️ Task deleted.");
    }
    else
    {
        Console.WriteLine("❌ Error: Please enter a valid number.");
    }
}

static void Main(string[] args)
{
    bool running = true;
    while (running)
    {
        Console.WriteLine("\n--- To-Do List Menu ---");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. View Tasks");
        Console.WriteLine("3. Complete Task");
        Console.WriteLine("4. Delete Task");
        Console.WriteLine("5. Exit");
        Console.Write("Choose an option (1-5): ");
        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                AddTask();
                break;
            case "2":
                ViewTasks();
                break;
            case "3":
                CompleteTask();
                break;
            case "4":
                DeleteTask();
                break;
            case "5":
                running = false;
                Console.WriteLine("👋 Exiting program...");
                break;
            default:
                Console.WriteLine("❌ Invalid option. Try again.");
                break;
        }
    }
}
```

Run the Visual Studio Code console application to check your answer:

```bash
dotnet run
```

If you receive an error when you run the code, go to the reading on the next page to compare
your code to the correct answer.

### Step 7 — Debugging techniques

The `CompleteTask` method from Step 5 has a hidden bug: if the user types a number that is
**out of range** (for example `7` when only two tasks exist), `tasks[taskNumber - 1]` throws an
`IndexOutOfRangeException` and the program crashes.

First, **introduce a debug line** that prints the number the user entered so you can see exactly
what value reaches the method. Then **handle the error gracefully** by validating the range and
showing the user a friendly message instead of crashing.

```csharp
static void CompleteTask()
{
    Console.Write("Enter the number of the task to mark as completed: ");
    string? input = Console.ReadLine();

    if (int.TryParse(input, out int taskNumber))
    {
        Console.WriteLine($"🛠 DEBUG: You entered task number {taskNumber}");

        if (taskNumber < 1 || taskNumber > taskCount)
        {
            Console.WriteLine("❌ Error: Invalid task number.");
            return;
        }

        tasks[taskNumber - 1] += " [Completed]";
        Console.WriteLine("✅ Task marked as completed.");
    }
    else
    {
        Console.WriteLine("❌ Error: Please enter a valid number.");
    }
}
```

Run the application again and try an out-of-range number. The `DEBUG` line confirms the value
that arrived, and the range check now returns a clean error message instead of throwing.

```bash
dotnet run
```

If you receive an error when you run the code, go to the reading on the next page to compare
your code to the correct answer.

> Tip: once the bug is fixed and confirmed, remove the `DEBUG: You entered…` line so it
> doesn't clutter the real output.

---

## ▶️ Expected result

The program shows the **To-Do List Menu** on a loop. You can add tasks, view the numbered list,
mark a task as completed (it shows `[Completed]`), and delete a task. Entering an out-of-range
or non-numeric task number now prints a clear error message instead of crashing, and choosing
**5** exits cleanly.

```text
--- To-Do List Menu ---
1. Add Task
2. View Tasks
3. Complete Task
4. Delete Task
5. Exit
Choose an option (1-5): 2

📋 To-Do List:
1. Buy groceries
2. Finish C# lab [Completed]
```

---

## ☑️ Definition of done

- [ ] `Program.cs` contains a `ToDoList` class with a `tasks` array (capacity 10) and `taskCount`
- [ ] `AddTask`, `ViewTasks`, `CompleteTask`, and `DeleteTask` are implemented
- [ ] `Main` shows the menu and loops until the user chooses **5. Exit**
- [ ] `CompleteTask` prints a `DEBUG` line and validates the task number range
- [ ] An out-of-range or non-numeric entry prints an error instead of crashing
- [ ] `dotnet run` builds and runs the menu end-to-end

---

## 🔑 Key concepts

- **Fixed-size arrays + a count** — a `string[10]` plus `taskCount` is a simple way to track
  how much of a fixed buffer is in use; always guard against exceeding `tasks.Length`.
- **Menu loop with a state flag** — a `while (running)` loop around a `switch` keeps the program
  interactive until the user explicitly exits.
- **Validate input before indexing** — `int.TryParse` rejects non-numbers, and a range check
  (`taskNumber < 1 || taskNumber > taskCount`) prevents `IndexOutOfRangeException`.
- **Debug by making the invisible visible** — printing the actual value that reaches a method
  (the `DEBUG` line) is the fastest way to confirm a bad input is the cause of a crash.
- **Fail gracefully** — return a clear, user-friendly error message instead of letting an
  unhandled exception terminate the program.
