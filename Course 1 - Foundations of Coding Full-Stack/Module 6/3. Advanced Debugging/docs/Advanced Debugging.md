# Advanced Debugging

**Course 1 — Foundations of Coding Full-Stack** · Module 6 · Lesson 3 · `You Try It!`

> Use **Microsoft Copilot** as a debugging partner on a working-but-fragile C# console
> To-Do app. You'll review the code by hand, then prompt Copilot to harden three weak
> spots — **loop efficiency**, **input validation**, and **edge-case handling** — and
> finish with a round of tests that prove the program no longer crashes or misbehaves.

---

## 🎯 Objective

Use Microsoft Copilot to identify and fix advanced issues in a code block, applying
AI-assisted debugging to sharpen your problem-solving. The focus is on **optimizing loops**,
**strengthening input validation**, and **handling edge cases** gracefully.

---

## 🧩 What you'll produce

A hardened version of a C# console **To-Do List** app, plus the Copilot prompts and
reasoning that got you there. You start from this buggy baseline and improve three areas:

| Area | Symptom in the starting code | Goal after debugging |
| ---- | ---------------------------- | -------------------- |
| **Loop efficiency** | `ViewTasks` re-checks state every iteration; no guard for empty input | Iterate cleanly, short-circuit on empty lists |
| **Input validation** | Menu choice parses, but blank/whitespace and out-of-range values slip through inconsistently | Reject non-numeric, out-of-range, and empty inputs early |
| **Edge cases** | Marking a non-existent task, empty task list, abrupt exit | Every branch degrades gracefully with a clear message |

**Debugging loop:** `Review by hand  →  Prompt Copilot  →  Apply suggestion  →  Re-test`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- An IDE with **Microsoft Copilot** available (Visual Studio or VS Code with the Copilot extension)
- The starting code below, loaded into a console project (`dotnet new console`)

---

## 🔍 Walkthrough — review the starting code

Before touching anything, read the program manually and locate the weak spots. This is the
code Copilot will help you harden:

```csharp
class Program
{
    static void Main()
    {
        List<string> tasks = new List<string>();
        bool exit = false;
        while (!exit)
        {
            ShowMenu();
            Console.Write("Enter your choice: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        ViewTasks(tasks);
                        break;
                    case 2:
                        AddTask(tasks);
                        break;
                    case 3:
                        MarkTaskComplete(tasks);
                        break;
                    case 4:
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
            Console.WriteLine();
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("=== TO-DO LIST MENU ===");
        Console.WriteLine("1. View Tasks");
        Console.WriteLine("2. Add Task");
        Console.WriteLine("3. Mark Task Complete");
        Console.WriteLine("4. Exit");
    }

    static void ViewTasks(List<string> tasks)
    {
        Console.WriteLine("\nYour Tasks:");
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
        }
        else
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i]}");
            }
        }
    }

    static void AddTask(List<string> tasks)
    {
        Console.Write("Enter the task: ");
        string? input = Console.ReadLine();
        string task = input != null ? input.Trim() : string.Empty;
        if (string.IsNullOrEmpty(task))
        {
            Console.WriteLine("Task cannot be empty.");
        }
        else
        {
            tasks.Add(task);
            Console.WriteLine("Task added.");
        }
    }

    static void MarkTaskComplete(List<string> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks to mark complete.");
            return;
        }
        Console.Write("Enter the task number to mark complete: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) &&
            taskNumber > 0 && taskNumber <= tasks.Count)
        {
            string currentTask = tasks[taskNumber - 1];
            if (currentTask.EndsWith(" [Complete]"))
            {
                Console.WriteLine("This task is already marked as complete.");
            }
            else
            {
                tasks[taskNumber - 1] += " [Complete]";
                Console.WriteLine("Task marked as complete.");
            }
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }
}
```

What to look for as you read:

- **Loops** — `ViewTasks` is the main loop. Is the empty-list check in the right place, and does the index math stay in bounds?
- **Validation** — the menu accepts any parsed `int`, but does every method reject blank or whitespace-only input the same way?
- **Edge cases** — what happens on an empty list, an out-of-range task number, or a task that's already complete?

---

## 🛠️ Steps

### Step 1 — Activate Copilot and surface logical errors

Load the code into your IDE with Microsoft Copilot active, then ask it to critique the
loop and point out problems you may have missed — especially how the loop processes each
item in the list.

> **Prompt:** *"Copilot, suggest improvements for this loop to handle all task scenarios."*

Use Copilot's response to confirm or extend your manual review: inefficient iterations,
validation gaps, and redundant operations.

### Step 2 — Optimize the loops

Focus on the loop structure next. Ask Copilot to help you rewrite it so it handles every
item without unnecessary iterations and short-circuits on empty or near-boundary lists.

> **Prompt:** *"How can I optimize this loop for better performance?"*

A clean, guarded display loop:

```csharp
static void ViewTasks(List<string> tasks)
{
    Console.WriteLine("\nYour Tasks:");
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks available.");
        return;                       // short-circuit: nothing to iterate
    }

    for (int i = 0; i < tasks.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {tasks[i]}");
    }
}
```

### Step 3 — Strengthen input validation

Ask Copilot to find weak points in the validation logic — non-numeric input,
out-of-range values, and empty or whitespace-only strings — then apply and test its
suggestions against each invalid scenario.

> **Prompt:** *"Can you suggest how to strengthen this input validation block?"*

Make blank/whitespace rejection consistent with `IsNullOrWhiteSpace`, and centralize
numeric parsing:

```csharp
static int ReadMenuChoice()
{
    Console.Write("Enter your choice: ");
    string? line = Console.ReadLine();
    if (int.TryParse(line, out int choice))
        return choice;

    Console.WriteLine("Please enter a valid number.");
    return -1;                        // sentinel handled by the default branch
}

static void AddTask(List<string> tasks)
{
    Console.Write("Enter the task: ");
    string task = (Console.ReadLine() ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(task))   // rejects "" and "   "
    {
        Console.WriteLine("Task cannot be empty.");
        return;
    }

    tasks.Add(task);
    Console.WriteLine("Task added.");
}
```

### Step 4 — Handle edge cases

Ask Copilot how to better manage unusual inputs and unexpected actions: no tasks present,
marking a task that doesn't exist, or exiting unexpectedly. Implement its suggestions so
every branch fails gracefully.

> **Prompt:** *"How should this code handle edge cases like marking a non-existent task or an empty list?"*

```csharp
static void MarkTaskComplete(List<string> tasks)
{
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks to mark complete.");
        return;                       // edge case: empty list
    }

    Console.Write("Enter the task number to mark complete: ");
    if (!int.TryParse(Console.ReadLine(), out int taskNumber) ||
        taskNumber < 1 || taskNumber > tasks.Count)
    {
        Console.WriteLine("Invalid task number.");   // edge case: non-existent / out-of-range
        return;
    }

    int index = taskNumber - 1;
    if (tasks[index].EndsWith(" [Complete]"))
    {
        Console.WriteLine("This task is already marked as complete.");
        return;                       // edge case: already complete
    }

    tasks[index] += " [Complete]";
    Console.WriteLine("Task marked as complete.");
}
```

### Step 5 — Final testing and validation

After applying Copilot's suggestions, run the program through several rounds of tests:

- Test with **valid and invalid inputs** (letters, blanks, out-of-range numbers).
- Confirm the loop runs efficiently for both **small and large** task lists.
- Confirm validation **prevents crashes or incorrect behavior** in every branch.

```bash
dotnet run
```

---

## ▶️ Expected result

The app runs the same To-Do menu, but now every path is safe: empty lists short-circuit,
non-numeric and out-of-range entries are rejected with a clear message, and marking a
missing or already-complete task no longer breaks the flow. No input crashes the program.

```text
=== TO-DO LIST MENU ===
1. View Tasks
2. Add Task
3. Mark Task Complete
4. Exit
Enter your choice: abc
Please enter a valid number.

=== TO-DO LIST MENU ===
...
Enter your choice: 3
No tasks to mark complete.
```

---

## ☑️ Definition of done

- [ ] You reviewed the starting code **by hand** and listed the weak spots before prompting Copilot
- [ ] Copilot's loop suggestions applied — `ViewTasks` short-circuits on empty lists and iterates cleanly
- [ ] Input validation rejects **non-numeric, out-of-range, and empty/whitespace** inputs consistently
- [ ] Edge cases handled — empty list, non-existent task number, already-complete task
- [ ] `dotnet run` completes a full test pass with valid and invalid inputs and **no crashes**

---

## 🔑 Key concepts

- **AI-assisted debugging is a dialogue** — Copilot surfaces candidates (inefficient loops, validation gaps), but you stay the reviewer: verify each suggestion against the program's intent before applying it.
- **Guard clauses and early return** — short-circuiting on empty lists or invalid input keeps loops efficient and flattens nesting, making each method easier to reason about.
- **Validate at the boundary** — parse and range-check every external input (`int.TryParse`, `IsNullOrWhiteSpace`, bounds on the task number) so bad data is rejected before it reaches your logic.
- **Edge cases are first-class** — empty collections, out-of-range indices, and "already done" states each need their own branch with a clear message, not a crash.
- **Test the unhappy paths** — a fix isn't done until you've driven invalid and boundary inputs through it and confirmed graceful behavior.
