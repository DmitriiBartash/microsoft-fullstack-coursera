# Practical Coding Improvement

**Course 2 — Introduction to Programming With C#** · Module 5 · Lesson 3 · `You Try It!`

> The Library Management System *works*, but it leans on five hard-coded `book1…book5`
> variables and a wall of repeated `if`/`else` checks. In this exercise you use
> **Microsoft Copilot** to refactor it: kill the duplication, tighten input validation,
> and fix the case-sensitivity bugs — without changing what the program does.

---

## 🎯 Objective

Use **Microsoft Copilot** as a pair-programmer to improve an existing C# console program's
readability, remove repetition, and make it more efficient — then confirm the refactored
version behaves the same as the original, only cleaner.

You will:

- Remove code duplication.
- Improve input validation.
- Address case-sensitivity issues for entering and removing books.

---

## 🧾 What you will produce

A refactored `LibraryManager` that replaces the five `string book1…book5` fields with a
single collection and a handful of small, single-purpose methods.

| Smell in the starting code | Refactor applied |
| --- | --- |
| Five separate `book1…book5` variables | One `List<Book?>` sized to a `MaxBooks` capacity |
| Repeated `if (string.IsNullOrEmpty(bookN))` chains for add/remove/display | `Add`/`Remove`/`Display` helper methods that loop over the list |
| `removeBook == bookN` — case-**sensitive** match | `StringComparison.OrdinalIgnoreCase` comparisons |
| No guard on empty/whitespace titles | `GetValidatedInput()` rejects blank input |
| Fixed capacity of 5, baked into the code | `GetLibrarySize()` prompts for capacity (defaults to 5) |
| String literals compared inline (`"add"`, `"remove"`, `"exit"`) | A `Command` enum + `ParseCommand()` |

**Flow:** `PrintMenu  →  ParseCommand(input)  →  Add / Remove / Exit  →  DisplayBooks  →  loop`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the **C# Dev Kit** extension
- **Microsoft Copilot** (or GitHub Copilot Chat) available to ask for suggestions
- The console application you created at the start of the course

---

## 🔎 Walkthrough

### Step 1 — Run the starting code to confirm it works

Open your console application, clear `Program.cs`, and paste in the **starting code**. Run it
and verify the behavior (add a book, remove a book, list books, exit) before changing anything —
you need a known-good baseline to refactor against.

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
            string action = Console.ReadLine().ToLower();
            if (action == "add")
            {
                if (!string.IsNullOrEmpty(book1) && !string.IsNullOrEmpty(book2) && !string.IsNullOrEmpty(book3) && !string.IsNullOrEmpty(book4) && !string.IsNullOrEmpty(book5))
                {
                    Console.WriteLine("The library is full. No more books can be added.");
                }
                else
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
                }
            }
            else if (action == "remove")
            {
                if (string.IsNullOrEmpty(book1) && string.IsNullOrEmpty(book2) && string.IsNullOrEmpty(book3) && string.IsNullOrEmpty(book4) && string.IsNullOrEmpty(book5))
                {
                    Console.WriteLine("The library is empty. No books to remove.");
                }
                else
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
            if (!string.IsNullOrEmpty(book1)) Console.WriteLine(book1);
            if (!string.IsNullOrEmpty(book2)) Console.WriteLine(book2);
            if (!string.IsNullOrEmpty(book3)) Console.WriteLine(book3);
            if (!string.IsNullOrEmpty(book4)) Console.WriteLine(book4);
            if (!string.IsNullOrEmpty(book5)) Console.WriteLine(book5);
        }
    }
}
```

### Step 2 — Ask Copilot to suggest improvements

Paste the starting code into **Microsoft Copilot** and ask it to reduce repetition and improve
readability. Useful prompts:

- *"Refactor this so the five book variables become a single collection."*
- *"Extract the add and remove logic into separate methods."*
- *"Make the book lookup case-insensitive."*

Copilot will typically suggest replacing `book1…book5` with a `List`, and pulling the repeated
add/remove/display blocks into helper methods.

### Step 3 — Simplify the code

Apply the suggestions: introduce a collection, extract helper methods, give variables clear
names, and fix the case-sensitivity bug. A `Book` record/class makes the intent explicit, a
`Command` enum removes the magic strings, and `StringComparison.OrdinalIgnoreCase` makes removal
case-insensitive. The full refactored program is below.

```csharp
using System.Globalization;

class Book(string title)
{
    public string Title { get; } = title;
    public override string ToString() => Title;
}

enum Command
{
    Add,
    Remove,
    Exit,
    Invalid
}

class LibraryManager
{
    private static int MaxBooks;
    private static List<Book?>? books;

    static void Main()
    {
        MaxBooks = GetLibrarySize();
        books = [.. new Book?[MaxBooks]];
        while (true)
        {
            PrintMenu();
            string? actionInput = Console.ReadLine()?.Trim().ToLowerInvariant();
            switch (ParseCommand(actionInput))
            {
                case Command.Add:
                    AddBook();
                    break;
                case Command.Remove:
                    RemoveBook();
                    break;
                case Command.Exit:
                    Console.WriteLine("Exiting Library Manager...");
                    return;
                default:
                    Console.WriteLine("Invalid input. Please enter 'add', 'remove', or 'exit'.");
                    break;
            }
            DisplayBooks();
        }
    }

    private static int GetLibrarySize()
    {
        Console.Write("Enter the maximum number of books in the library: ");
        if (int.TryParse(Console.ReadLine(), out int size) && size > 0)
            return size;
        Console.WriteLine("Invalid number. Defaulting to 5.");
        return 5;
    }

    private static Command ParseCommand(string? input) => input switch
    {
        "add" => Command.Add,
        "remove" => Command.Remove,
        "exit" => Command.Exit,
        _ => Command.Invalid
    };

    private static void PrintMenu()
    {
        Console.WriteLine("\nLibrary Manager");
        Console.WriteLine($"Books stored: {(books ?? []).Count(b => b != null)} / {MaxBooks}");
        Console.Write("Choose an action (add / remove / exit): ");
    }

    private static void AddBook()
    {
        if (books == null || !books.Any(b => b == null))
        {
            Console.WriteLine("The library is full. Cannot add more books.");
            return;
        }
        string? title = GetValidatedInput("Enter the book title to add: ");
        if (title == null) return;
        if (BookExists(title))
        {
            Console.WriteLine("This book already exists in the library.");
            return;
        }
        int index = books.FindIndex(b => b == null);
        books[index] = new Book(title);
        Console.WriteLine($"Book \"{title}\" added to the library.");
    }

    private static void RemoveBook()
    {
        if (books == null || books.All(b => b == null))
        {
            Console.WriteLine("The library is empty. No books to remove.");
            return;
        }
        string? title = GetValidatedInput("Enter the book title to remove: ");
        if (title == null) return;
        int index = books.FindIndex(b => b != null && b!.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (index >= 0)
        {
            books[index] = null;
            Console.WriteLine($"Book \"{title}\" removed from the library.");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
    }

    private static void DisplayBooks()
    {
        Console.WriteLine("\nCurrent books in the library:");
        var nonEmptyBooks = (books ?? []).Where(b => b != null).Select(b => b!.Title).ToList();
        if (nonEmptyBooks.Count == 0)
        {
            Console.WriteLine("No books available.");
        }
        else
        {
            Console.WriteLine(string.Join("\n", nonEmptyBooks.Select(b => "- " + b)));
        }
    }

    private static string? GetValidatedInput(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Book title cannot be empty.");
            return null;
        }
        return NormalizeTitle(input);
    }

    private static string NormalizeTitle(string title)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLowerInvariant());
    }

    private static bool BookExists(string title)
    {
        return books != null && books.Any(b => b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
}
```

### Step 4 — Test the simplified program

Clear `Program.cs` again, paste in the refactored code, and run it:

```bash
dotnet run
```

Exercise the same paths as before — add, remove (try different casing this time), list, and exit.
The behavior should match the original, but the code is now far cleaner. When it works, **save your
code: you will reuse it for the final project in this course.**

---

## ▶️ Expected result

The refactored app behaves like the original — add, remove, list, exit — but now:

- adding `"The Hobbit"` and later removing `"the hobbit"` **works**, because matching is case-insensitive;
- blank/whitespace titles are rejected instead of silently stored;
- the menu shows a live `Books stored: n / MaxBooks` count;
- there is no `book1…book5` duplication left to maintain.

```text
Enter the maximum number of books in the library: 3

Library Manager
Books stored: 0 / 3
Choose an action (add / remove / exit): add
Enter the book title to add: the hobbit
Book "The Hobbit" added to the library.

Current books in the library:
- The Hobbit
```

---

## ☑️ Definition of done

- [ ] The starting code runs and its behavior is verified before refactoring.
- [ ] The five `book1…book5` variables are replaced by a single collection.
- [ ] Add, remove, and display logic live in dedicated helper methods (no repeated `if` chains).
- [ ] Book matching is **case-insensitive** (`StringComparison.OrdinalIgnoreCase`).
- [ ] Empty/whitespace titles are rejected by validation.
- [ ] The refactored program runs via `dotnet run` and behaves the same as the original.
- [ ] The final code is saved for reuse in the course's final project.

---

## 🔑 Key concepts

- **Remove duplication (DRY)** — five parallel `book1…book5` variables and their copy-pasted
  `if`/`else` blocks become one `List<Book?>` plus small helper methods you write once.
- **Validate at the boundary** — `GetValidatedInput()` trims and rejects blank input in one place,
  so no downstream code has to re-check for empty titles.
- **Case-insensitive comparison** — comparing titles with `StringComparison.OrdinalIgnoreCase`
  fixes the bug where `"hobbit"` would not match a stored `"Hobbit"`.
- **Name things, drop magic strings** — a `Command` enum and `ParseCommand()` replace bare
  `"add"`/`"remove"`/`"exit"` literals, making the control flow self-documenting.
- **Refactor behind a safety net** — verify the original works *first*, then change structure only;
  the program's observable behavior must stay the same.
