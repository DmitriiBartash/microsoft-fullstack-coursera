# Creating a Task Manager

**Course 1 — Foundations of Coding Full-Stack** · Module 6 · Lesson 1 · `You Try It!`

> Build a small C# console **task manager** that stores up to three to-do items in
> plain string variables and tracks each one's status with a Boolean flag. The point
> of the lab is to solve a real problem using only the fundamentals you already know —
> **variables, `if`/`else` conditionals, Boolean logic, and a loop** — *before* arrays
> enter the picture.

---

## 🎯 Objective

You will create a basic task manager program that stores and displays up to three tasks
using individual `string` variables. Tasks can be marked as completed using **Boolean
flags**, and the program displays which tasks are completed or still pending — all driven
by `if`/`else` decisions inside a menu loop.

---

## 🗂️ What you will build

A single-file console app, `TaskManager`, with a menu loop that routes the user to small
helper methods:

| Piece                             | Responsibility                                             |
| --------------------------------- | ---------------------------------------------------------- |
| `task1`, `task2`, `task3`         | Three `string` slots that hold the task descriptions       |
| `isTask1Completed` … `3Completed` | Three `bool` flags tracking each task's status             |
| `ShowMenu()`                      | Print the options and prompt for a choice                  |
| `AddTask()` / `GetTaskDescription()` | Store a new task in the first empty slot                |
| `MarkTaskAsCompleted()`           | Flip a task's Boolean flag to `true`                       |
| `RemoveTask()`                    | Clear a task slot and reset its flag                       |
| `ShowTasks()` / `ShowTask(...)`   | Print each task with `Completed` / `Pending` / `[Empty]`   |

**Flow:** `Main loop  →  ShowMenu()  →  read choice  →  switch  →  Add / Mark / Show / Remove / Exit`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor such as Visual Studio Code
- Comfort with C# `string` and `bool` variables, `if`/`else`, and a `while` loop

---

## 🛠️ Steps

### Step 1 — Create the project

Scaffold a new console app and move into it.

```bash
dotnet new console -n TaskManager
cd TaskManager
```

Open `Program.cs` and **clear any existing content** — you will replace it with the code below.

### Step 2 — Create variables for tasks and their status

Inside the class, declare three `string` variables to hold the tasks and three `bool`
variables to track whether each task is completed. Initialise the strings to empty (`""`)
so an empty slot is easy to detect, and the flags to `false`.

```csharp
// Task descriptions and statuses
static string task1 = "", task2 = "", task3 = "";
static bool isTask1Completed = false, isTask2Completed = false, isTask3Completed = false;
```

### Step 3 — Prompt the user to add a task

Ask the user for a task, then check which slot is empty and store the new task there.
If all three slots are full, tell the user no more tasks can be added.

```csharp
static void AddTask()
{
    if (task1 == "")
    {
        task1 = GetTaskDescription();
        Console.WriteLine("Task 1 added.");
    }
    else if (task2 == "")
    {
        task2 = GetTaskDescription();
        Console.WriteLine("Task 2 added.");
    }
    else if (task3 == "")
    {
        task3 = GetTaskDescription();
        Console.WriteLine("Task 3 added.");
    }
    else
    {
        Console.WriteLine("Task list is full. You can't add more than 3 tasks.");
    }
}

static string GetTaskDescription()
{
    Console.Write("Enter task description: ");
    return Console.ReadLine() ?? "";
}
```

### Step 4 — Mark a task as completed

Ask which task (1, 2, or 3) to complete, then use `if`/`else` conditions to flip the
matching Boolean flag to `true` and confirm. If the number is invalid (or the task does
not exist), display an error message instead.

```csharp
static void MarkTaskAsCompleted()
{
    Console.Write("Enter task number to mark as completed (1-3): ");
    string taskNum = Console.ReadLine() ?? "";
    if (taskNum == "1" && task1 != "")
        isTask1Completed = true;
    else if (taskNum == "2" && task2 != "")
        isTask2Completed = true;
    else if (taskNum == "3" && task3 != "")
        isTask3Completed = true;
    else
    {
        Console.WriteLine("Invalid task number or task does not exist.");
        return;
    }
    Console.WriteLine($"Task {taskNum} marked as completed.");
}
```

### Step 5 — Display the tasks and their status

Print each task with its status. Use an `if`/`else` (here, a conditional expression) to
decide between `Completed` and `Pending`, and show `[Empty]` for slots with no task yet.

```csharp
static void ShowTasks()
{
    Console.WriteLine("\n--- Task List ---");
    ShowTask(1, task1, isTask1Completed);
    ShowTask(2, task2, isTask2Completed);
    ShowTask(3, task3, isTask3Completed);
}

static void ShowTask(int number, string task, bool isCompleted)
{
    if (task == "")
        Console.WriteLine($"Task {number}: [Empty]");
    else
        Console.WriteLine($"Task {number}: {task} - {(isCompleted ? "Completed" : "Pending")}");
}
```

### Step 6 — Add a remove (clean) option

Let the user free a slot by entering its number. Clear the `string` back to `""` and reset
its Boolean flag so the slot can be reused; otherwise show an error.

```csharp
static void RemoveTask()
{
    Console.Write("Enter task number to remove (1-3): ");
    string taskNum = Console.ReadLine() ?? "";
    if (taskNum == "1" && task1 != "")
    {
        task1 = "";
        isTask1Completed = false;
        Console.WriteLine("Task 1 removed.");
    }
    else if (taskNum == "2" && task2 != "")
    {
        task2 = "";
        isTask2Completed = false;
        Console.WriteLine("Task 2 removed.");
    }
    else if (taskNum == "3" && task3 != "")
    {
        task3 = "";
        isTask3Completed = false;
        Console.WriteLine("Task 3 removed.");
    }
    else
    {
        Console.WriteLine("Invalid task number or task does not exist.");
    }
}
```

### Step 7 — Wire it together in `Main` and run

Drive everything from a `while` loop: show the menu, read the choice, and use a `switch`
to call the right method. Option `4` ends the loop.

```csharp
using System;

namespace TaskManager
{
    class TaskManager
    {
        // Task descriptions and statuses
        static string task1 = "", task2 = "", task3 = "";
        static bool isTask1Completed = false, isTask2Completed = false, isTask3Completed = false;

        static void Main()
        {
            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        MarkTaskAsCompleted();
                        break;
                    case "3":
                        ShowTasks();
                        break;
                    case "4":
                        running = false;
                        Console.WriteLine("Exiting Task Manager. Goodbye!");
                        break;
                    case "5":
                        RemoveTask();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n=== Task Manager ===");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Completed");
            Console.WriteLine("3. Show Tasks");
            Console.WriteLine("4. Exit");
            Console.WriteLine("5. Remove (Clean) a Task");
            Console.Write("Choose an option (1-5): ");
        }

        // ... AddTask, GetTaskDescription, MarkTaskAsCompleted,
        //     ShowTasks, ShowTask, and RemoveTask from the steps above
    }
}
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The menu loops until you choose **4 (Exit)**. A typical session:

```text
=== Task Manager ===
1. Add Task
2. Mark Task as Completed
3. Show Tasks
4. Exit
5. Remove (Clean) a Task
Choose an option (1-5): 1
Enter task description: Write unit tests
Task 1 added.

=== Task Manager ===
...
Choose an option (1-5): 2
Enter task number to mark as completed (1-3): 1
Task 1 marked as completed.

=== Task Manager ===
...
Choose an option (1-5): 3

--- Task List ---
Task 1: Write unit tests - Completed
Task 2: [Empty]
Task 3: [Empty]
```

---

## ☑️ Definition of done

- [ ] `TaskManager` console project created and `Program.cs` cleared
- [ ] Three `string` task slots and three `bool` status flags declared
- [ ] `AddTask()` fills the first empty slot and rejects a 4th task when full
- [ ] `MarkTaskAsCompleted()` flips the right flag and errors on an invalid number
- [ ] `ShowTasks()` prints `Completed`, `Pending`, or `[Empty]` for each slot
- [ ] `RemoveTask()` clears a slot and resets its flag
- [ ] `dotnet run` shows the menu loop and exits cleanly on option `4`

---

## 🔑 Key concepts

- **Boolean flags as state** — a single `bool` per task captures "done vs not done", and a
  conditional expression turns that flag into the words `Completed` / `Pending`.
- **`if`/`else` chains for selection** — checking slots in order (`task1` empty? else `task2`?
  …) is how you route work before you know about arrays or collections.
- **Guard before you act** — every action validates first (`taskNum == "2" && task2 != ""`),
  so the program fails gracefully on bad input instead of corrupting state.
- **A menu loop with `switch`** — a `while (running)` loop plus a `switch` on the choice is a
  classic, readable shape for a console app, with `running = false` as the clean exit.
- **Small, single-purpose methods** — `AddTask`, `ShowTask`, `RemoveTask` each do one thing,
  keeping `Main` short and the logic easy to follow.
