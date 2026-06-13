# Practical Debugging with Copilot

**Course 2 — Introduction to Programming With C#** · Module 5 · Lesson 2 · `You Try It!`

> Take a working-but-buggy **Library Management System** console app and fix it with the
> help of **Microsoft Copilot**. The starting code can't reliably add, remove, or display
> books — your job is to spot the defects, ask Copilot for fixes, apply them, and confirm
> the program behaves correctly.

---

## 🎯 Objective

Practice a real debugging workflow in C#: run code that misbehaves, **identify the root
causes**, use **Microsoft Copilot** to suggest corrections, apply the fixes, and **verify**
the repaired program adds, removes, and displays books exactly as intended.

---

## 🐞 What you'll fix

The starting code compiles, but three defects break its behavior:

| # | Defect | Why it's a problem |
| - | ------ | ------------------ |
| 1 | Books are added **before** checking whether the library is full | A 6th title can overwrite logic or be silently dropped; the "library full" message fires inconsistently |
| 2 | The display loop prints **every** slot, including empty ones | Blank lines are printed for unused slots, confusing the reader |
| 3 | `action` is compared without normalizing case | Typing `Add` or `REMOVE` is treated as an invalid command |

**Flow:** `Run buggy code  →  observe issues  →  ask Copilot  →  apply fixes  →  re-test`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the C# extension
- Access to **Microsoft Copilot**
- The console application you created at the start of the course (edit its `Program.cs`)

---

## 🐛 Starting code (with errors)

Replace everything in `Program.cs` with the code below, run it, and watch for the defects.

```csharp
class LibraryManager
{
    static void Main()
    {
        string book1 = "";
        string book2 = "";
        string book3 = "";
        string book4 = "";
        string book5 = "";
        while (true)
        {
            Console.WriteLine("Would you like to add or remove a book? (add/remove/exit)");
            string action = Console.ReadLine();
            if (action == "add")
            {
                Console.WriteLine("Enter the title of the book to add:");
                string newBook = Console.ReadLine();
                if (string.IsNullOrEmpty(book1))
                {
                    book1 = newBook;
                }
                else if (string.IsNullOrEmpty(book2))
                {
                    book2 = newBook;
                }
                else if (string.IsNullOrEmpty(book3))
                {
                    book3 = newBook;
                }
                else if (string.IsNullOrEmpty(book4))
                {
                    book4 = newBook;
                }
                else if (string.IsNullOrEmpty(book5))
                {
                    book5 = newBook;
                }
                else
                {
                    Console.WriteLine("The library is full. No more books can be added.");
                }
            }
            else if (action == "remove")
            {
                Console.WriteLine("Enter the title of the book to remove:");
                string removeBook = Console.ReadLine();
                if (removeBook == book1)
                {
                    book1 = "";
                }
                else if (removeBook == book2)
                {
                    book2 = "";
                }
                else if (removeBook == book3)
                {
                    book3 = "";
                }
                else if (removeBook == book4)
                {
                    book4 = "";
                }
                else if (removeBook == book5)
                {
                    book5 = "";
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else if (action == "exit")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'add', 'remove', or 'exit'.");
            }
            // Display the list of books
            Console.WriteLine("Available books:");
            Console.WriteLine(book1);
            Console.WriteLine(book2);
            Console.WriteLine(book3);
            Console.WriteLine(book4);
            Console.WriteLine(book5);
        }
    }
}
```

---

## 🔎 Walkthrough

### Step 1 — Identify the errors

Run the **Starting code (with errors)** and observe the issues:

- **No "full" check up front when adding.** Each `add` walks an `if`/`else if` chain over
  five separate variables. The logic is brittle and the "library is full" branch only runs
  after all five checks fall through — easy to get wrong as the code grows.
- **Empty slots are printed.** The display block writes `book1`…`book5` unconditionally, so
  unused slots show up as blank lines and clutter the output.
- **Case-sensitive command matching.** `action == "add"` only matches lowercase input. If the
  user types `Add`, the program treats it as an invalid command.

### Step 2 — Use Copilot to debug

Ask **Microsoft Copilot** to identify and suggest fixes for the errors — for example:

> *"This C# console app should add, remove, and display books, but empty slots get printed,
> commands are case-sensitive, and the full-library check is unreliable. Find the bugs and
> suggest fixes."*

Apply the suggested corrections. The key improvements are:

- Store the titles in a `string[] books` array and **loop** instead of five named variables.
- **Check for free space before adding**, and skip the rest of the loop with `continue`
  when the library is full.
- Normalize input with `Console.ReadLine()?.Trim().ToLower()` so `Add`, `ADD`, and `add`
  all match.
- When displaying, **skip null or empty entries** so only real titles are shown.

### Step 3 — Test the debugged program

After making the corrections, test the program by **adding**, **removing**, and **displaying**
books, including the edge cases: a full library, removing a title that doesn't exist, and
mixed-case commands.

---

## 🛠️ Corrected code

Replace the contents of `Program.cs` with the debugged version:

```csharp
class LibraryManager
{
    static void Main()
    {
        string[] books = new string[5];
        while (true)
        {
            Console.WriteLine("Would you like to add or remove a book? (add/remove/exit)");
            string action = Console.ReadLine()?.Trim().ToLower();
            if (action == "add")
            {
                bool hasSpace = false;
                foreach (var book in books)
                {
                    if (string.IsNullOrEmpty(book))
                    {
                        hasSpace = true;
                        break;
                    }
                }
                if (!hasSpace)
                {
                    Console.WriteLine("The library is full. No more books can be added.");
                    continue;
                }
                Console.WriteLine("Enter the title of the book to add:");
                string newBook = Console.ReadLine()?.Trim();
                for (int i = 0; i < books.Length; i++)
                {
                    if (string.IsNullOrEmpty(books[i]))
                    {
                        books[i] = newBook;
                        Console.WriteLine($"Book \"{newBook}\" added.");
                        break;
                    }
                }
            }
            else if (action == "remove")
            {
                Console.WriteLine("Enter the title of the book to remove:");
                string removeBook = Console.ReadLine()?.Trim();
                bool found = false;
                for (int i = 0; i < books.Length; i++)
                {
                    if (books[i] == removeBook)
                    {
                        books[i] = "";
                        Console.WriteLine($"Book \"{removeBook}\" removed.");
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else if (action == "exit")
            {
                Console.WriteLine("Exiting program...");
                break;
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'add', 'remove', or 'exit'.");
            }
            // Display the list of books
            Console.WriteLine("\nAvailable books:");
            foreach (var book in books)
            {
                if (!string.IsNullOrEmpty(book))
                {
                    Console.WriteLine($"- {book}");
                }
            }
            Console.WriteLine();
        }
    }
}
```

---

## ▶️ Expected result

The repaired program loops cleanly: it accepts `add`, `remove`, and `exit` regardless of
letter case, confirms each add/remove, refuses new titles once five books are stored, and
lists **only** the non-empty titles — no stray blank lines. Typing `exit` prints
`Exiting program...` and ends the loop.

---

## ☑️ Definition of done

- [ ] Starting code runs and you can name all three defects (full-check, empty-line display, case sensitivity)
- [ ] Copilot was used to identify and suggest the fixes
- [ ] `Program.cs` updated so titles are stored in a `string[] books` array
- [ ] Input is normalized with `Trim().ToLower()` so mixed-case commands work
- [ ] Adding is blocked once the library holds five books
- [ ] Display skips empty slots and shows only real titles
- [ ] Add, remove, full-library, and "book not found" cases all tested and behave correctly

---

## 🔑 Key concepts

- **Reproduce before you fix** — running the buggy code and observing the actual symptoms is
  the first debugging step; you can't fix what you haven't seen fail.
- **Data structures shape correctness** — swapping five ad-hoc variables for a `string[]`
  array lets a single loop handle add, remove, and display uniformly and removes whole
  classes of copy-paste bugs.
- **Validate and normalize input** — `Trim()` and `ToLower()` make command matching robust
  to whitespace and capitalization; defensive input handling prevents "phantom" invalid commands.
- **Guard clauses keep logic clear** — checking for a full library up front and using
  `continue` avoids deeply nested conditionals and makes the intent obvious.
- **Copilot is an assistant, not an oracle** — it accelerates spotting and fixing defects,
  but you still reproduce, read the suggestion, apply it deliberately, and re-test to confirm.
