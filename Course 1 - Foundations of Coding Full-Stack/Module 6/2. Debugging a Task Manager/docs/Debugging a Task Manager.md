# Debugging a Task Manager

**Course 1 — Foundations of Coding Full-Stack** · Module 6 · Lesson 2 · `You Try It!`

> A pre-written C# task manager *looks* fine but misbehaves at runtime: it crashes
> when you view tasks, marks the wrong task complete, and blows up on bad input.
> Your job is to **hunt down the logical errors** and fix them so tasks are stored,
> completed, and displayed correctly.

---

## 🎯 Objective

Debug a pre-existing task manager program. Identify and fix the **logical errors** in the
task-addition and task-completion functions so that tasks are stored correctly, marked as
completed accurately, and displayed without crashing — including the awkward boundary cases
(no tasks yet, an out-of-range number, non-numeric input).

---

## 🧩 What you will produce

A corrected version of the console `TaskManager` that survives every path through the menu:

| Area                | Bug to fix                                                              |
| ------------------- | ---------------------------------------------------------------------- |
| `ViewTasks()`       | Loop runs one index too far (`<=`) → `ArgumentOutOfRangeException`      |
| `CompleteTask()`    | Treats the user's **1-based** choice as a **0-based** index (off-by-one)|
| `CompleteTask()`    | `int.Parse` crashes on non-numeric input — needs `int.TryParse`        |
| Boundary cases      | No graceful handling for an **empty** task list                        |

**Flow:** `Menu → Add / Complete / View → validate input → update list → back to Menu`

---

## ✅ Prerequisites

- A C# runner — this lab uses **[.NET Fiddle](https://dotnetfiddle.net)** (no local install needed)
- Familiarity with C# `List<T>`, `for` loops, and `switch` statements
- The starter program below (it compiles, but contains the bugs)

---

## 🐞 Walkthrough

### The starter program (buggy)

This is the code you are given. Read it first and predict where it breaks.

```csharp
using System;
using System.Collections.Generic;

class TaskManager
{
    static List<string> tasks = new List<string>();
    static List<bool> taskStatus = new List<bool>();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Task Manager");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Completed");
            Console.WriteLine("3. View Tasks");
            Console.WriteLine("4. Exit");
            Console.WriteLine("What would you like to do? (choose 1-4)");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    CompleteTask();
                    break;
                case "3":
                    ViewTasks();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }

    static void AddTask()
    {
        Console.WriteLine("Enter task description:");
        string task = Console.ReadLine();
        tasks.Add(task);
        taskStatus.Add(false);  // Marking as not completed by default
        Console.WriteLine("Task added successfully.");
    }

    static void CompleteTask()
    {
        Console.WriteLine("Enter task number to mark as completed:");
        int taskNumber = int.Parse(Console.ReadLine());        // 💥 crashes on bad input
        if (taskNumber < 0 || taskNumber >= tasks.Count)       // 💥 wrong bounds (0-based vs 1-based)
        {
            Console.WriteLine("Invalid task number.");
            return;
        }
        taskStatus[taskNumber] = true;                         // 💥 off-by-one: marks the wrong task
        Console.WriteLine($"Task '{tasks[taskNumber]}' marked as completed.");
    }

    static void ViewTasks()
    {
        Console.WriteLine("Tasks:");
        for (int i = 0; i <= tasks.Count; i++)                 // 💥 <= runs past the last index
        {
            string status = taskStatus[i] ? "Completed" : "Pending";
            Console.WriteLine($"{i + 1}. {tasks[i]} - {status}");
        }
    }
}
```

### Bug 1 — `ViewTasks()` loops one index too far

The loop condition is `i <= tasks.Count`. For a list of 3 tasks the valid indices are `0, 1, 2`,
but `<=` lets `i` reach `3`, so `tasks[3]` throws `ArgumentOutOfRangeException` the moment you
try to view your tasks.

```csharp
// before
for (int i = 0; i <= tasks.Count; i++)
// after — stop one short of Count
for (int i = 0; i < tasks.Count; i++)
```

> The display already prints `{i + 1}` to show a friendly **1-based** number to the user.
> Remember that detail — it is the root of Bug 2.

### Bug 2 — `CompleteTask()` mixes 1-based and 0-based numbering

The menu shows tasks as `1. … 2. … 3. …`, so a user naturally types **1** to complete the first
task. But the code uses that value *directly* as a list index. Typing `1` actually marks
`taskStatus[1]` — the **second** task — and typing `3` (a valid display number for 3 tasks)
hits index `3`, which is out of range.

Fix the bounds check **and** convert the user's 1-based choice into a 0-based index:

```csharp
// validate against the 1..Count range the user actually sees
if (taskNumber < 1 || taskNumber > tasks.Count)
{
    Console.WriteLine("Invalid task number.");
    return;
}
int index = taskNumber - 1;        // convert 1-based → 0-based
taskStatus[index] = true;
Console.WriteLine($"Task '{tasks[index]}' marked as completed.");
```

### Bug 3 — bad input crashes the program

`int.Parse(Console.ReadLine())` throws a `FormatException` if the user types `abc` or just presses
Enter. The instructions call for `int.TryParse`, which returns `false` instead of throwing so you
can show a friendly message and return to the menu:

```csharp
if (!int.TryParse(Console.ReadLine(), out int taskNumber))
{
    Console.WriteLine("Please enter a valid number.");
    return;
}
```

### Bug 4 — empty-list boundary case

If the user picks **View** or **Mark Complete** before adding anything, the program should say so
rather than loop over nothing or read a number that can't be valid. Guard the empty case up front:

```csharp
if (tasks.Count == 0)
{
    Console.WriteLine("No tasks yet. Add one first.");
    return;
}
```

### The corrected program

Putting all four fixes together:

```csharp
using System;
using System.Collections.Generic;

class TaskManager
{
    static List<string> tasks = new List<string>();
    static List<bool> taskStatus = new List<bool>();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Task Manager");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Completed");
            Console.WriteLine("3. View Tasks");
            Console.WriteLine("4. Exit");
            Console.WriteLine("What would you like to do? (choose 1-4)");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    CompleteTask();
                    break;
                case "3":
                    ViewTasks();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }

            Console.WriteLine();   // blank line, then back to the menu
        }
    }

    static void AddTask()
    {
        Console.WriteLine("Enter task description:");
        string task = Console.ReadLine();
        tasks.Add(task);
        taskStatus.Add(false);  // not completed by default
        Console.WriteLine("Task added successfully.");
    }

    static void CompleteTask()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks yet. Add one first.");
            return;
        }

        Console.WriteLine("Enter task number to mark as completed:");
        if (!int.TryParse(Console.ReadLine(), out int taskNumber))
        {
            Console.WriteLine("Please enter a valid number.");
            return;
        }

        if (taskNumber < 1 || taskNumber > tasks.Count)
        {
            Console.WriteLine("Invalid task number.");
            return;
        }

        int index = taskNumber - 1;           // 1-based input → 0-based index
        taskStatus[index] = true;
        Console.WriteLine($"Task '{tasks[index]}' marked as completed.");
    }

    static void ViewTasks()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks to show.");
            return;
        }

        Console.WriteLine("Tasks:");
        for (int i = 0; i < tasks.Count; i++)  // strictly less than Count
        {
            string status = taskStatus[i] ? "Completed" : "Pending";
            Console.WriteLine($"{i + 1}. {tasks[i]} - {status}");
        }
    }
}
```

---

## 🚀 Your turn

Paste your corrected code into **.NET Fiddle** (delete the page's sample code, paste yours on the
left, and click **Run**), then exercise every path:

- Add several tasks, then **View** them — confirm the numbering starts at `1` and nothing crashes.
- **Mark complete** by number and re-view — the *correct* task should flip to `Completed`.
- Try invalid input: a letter, a negative number, `0`, and a number larger than the task count —
  each should produce a friendly message and return you to the menu.
- Pick **View** and **Mark Complete** with **zero tasks** — both should report there are none.
- Choose **Exit** and confirm the program ends cleanly.

---

## ▶️ Expected result

```text
Tasks:
1. Buy groceries - Completed
2. Write report - Pending
3. Call the bank - Pending
```

The program loops through the menu indefinitely, stores every task with its status, marks the
task the user *actually* chose, and handles empty lists and bad input without throwing.

---

## ☑️ Definition of done

- [ ] `ViewTasks()` loop uses `i < tasks.Count` and no longer throws on **View**
- [ ] `CompleteTask()` validates `1 ≤ number ≤ Count` and converts to a `number - 1` index
- [ ] Marking a task complete flips the **correct** task's status
- [ ] `int.TryParse` replaces `int.Parse`, so non-numeric input is handled gracefully
- [ ] Empty task list is handled in both **View** and **Mark Complete**
- [ ] Program returns to the main menu after each action and **Exit** ends it cleanly

---

## 🔑 Key concepts

- **Off-by-one errors** — `<=` vs `<` and 1-based vs 0-based indexing are the classic source of
  list bugs; a loop or index that is off by one either crashes or touches the wrong element.
- **Parallel lists stay in sync** — `tasks` and `taskStatus` share the same index, so every fix
  must keep their indices aligned when adding, reading, and updating.
- **Validate before you index** — check bounds (and parse safely with `int.TryParse`) *before*
  using a value as an index, so bad input fails gracefully instead of throwing.
- **Boundary cases are first-class** — empty collections and out-of-range numbers are normal user
  behaviour; handle them explicitly rather than letting the runtime decide.
- **Read the contract the UI promises** — because the menu shows `1, 2, 3`, the logic must accept
  `1, 2, 3`; mismatched assumptions between display and data cause subtle, hard-to-spot bugs.
