# Practical Coding Exercises

**Course 2 — Introduction to Programming With C#** · Module 5 · Lesson 4 · `Exercise`

> Use Microsoft Copilot to **extend the Library Management System** with three new
> features: **search** for a book by title, a **borrowing limit** (max 3 at a time),
> and **check-in** of borrowed books. Start from the code you optimized in the previous
> "You Try It!" activity — this is the code you'll submit as your **final project**.

---

## 🎯 Objective

You will use **Microsoft Copilot** to extend the Library Management System by solving new
challenges — searching for a book and limiting how many books a user can borrow. Build on
the console app you already optimized in the previous activity, and save the finished
program as the **final project** for this course.

---

## 🗂️ What you will build

A single C# console app, `LibraryManager`, that grows three new menu commands on top of the
existing **add / remove / display** workflow.

| Feature | Command | Behavior |
| ------- | ------- | -------- |
| Search | `search` | Prompt for a title; report whether the book is in the collection |
| Borrow | `borrow` | Mark a book borrowed; refuse once the limit of **3** is reached |
| Check-in | `checkin` | Clear the borrowed flag so the book can be borrowed again |

**Flow:** `Menu → ParseCommand(input) → Add / Remove / Search / Borrow / CheckIn / Exit → DisplayBooks`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (with the Microsoft Copilot / C# extensions)
- The Library Management System console app from the previous **"You Try It!"** activity

---

## 🛠️ Steps

### Step 1 — Add a search feature

Let the user look up a book by title.

- Prompt the user to input a book title to search for.
- If the book is found, display a message indicating it is available.
- If the book is not found, display a message that it's not in the collection.

```csharp
private static void SearchBook()
{
    string? title = GetValidatedInput("Enter the book title to search: ");
    if (title == null) return;

    var book = books.FirstOrDefault(b =>
        b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

    if (book != null)
        Console.WriteLine($"Book found: {book}");
    else
        Console.WriteLine("Book not found in the library.");
}
```

### Step 2 — Limit borrowing

Track how many books are checked out and cap it.

- Add a feature that tracks how many books a user has borrowed.
- Limit the number of books to **3** at a time.

```csharp
private const int BorrowLimit = 3;
private static int BorrowedCount = 0;

private static void BorrowBook()
{
    if (BorrowedCount >= BorrowLimit)
    {
        Console.WriteLine($"You cannot borrow more than {BorrowLimit} books at a time.");
        return;
    }

    string? title = GetValidatedInput("Enter the book title to borrow: ");
    if (title == null) return;

    var book = books.FirstOrDefault(b =>
        b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

    if (book == null)
        Console.WriteLine("Book not found.");
    else if (book.IsBorrowed)
        Console.WriteLine("That book is already borrowed.");
    else
    {
        book.IsBorrowed = true;
        BorrowedCount++;
        Console.WriteLine($"You have borrowed \"{book.Title}\".");
    }
}
```

### Step 3 — Check in a borrowed book

Let the user return a book that's currently checked out.

- Add a feature that enables the user to check in a book that's been checked out.
- Check if the book is checked out. If it is, remove the checked-out flag to check the book in. If it isn't, inform the user.

```csharp
private static void CheckInBook()
{
    string? title = GetValidatedInput("Enter the book title to check in: ");
    if (title == null) return;

    var book = books.FirstOrDefault(b =>
        b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

    if (book == null)
        Console.WriteLine("Book not found.");
    else if (!book.IsBorrowed)
        Console.WriteLine("That book is not currently borrowed.");
    else
    {
        book.IsBorrowed = false;
        BorrowedCount--;
        Console.WriteLine($"You have checked in \"{book.Title}\".");
    }
}
```

### Step 4 — Wire the commands into the menu

Extend the `Command` enum, the parser, and the main `switch` so the new actions are reachable.

```csharp
enum Command { Add, Remove, Search, Borrow, CheckIn, Exit, Invalid }

private static Command ParseCommand(string? input) => input switch
{
    "add"     => Command.Add,
    "remove"  => Command.Remove,
    "search"  => Command.Search,
    "borrow"  => Command.Borrow,
    "checkin" => Command.CheckIn,
    "exit"    => Command.Exit,
    _         => Command.Invalid
};

// inside Main's loop:
switch (ParseCommand(actionInput))
{
    case Command.Add:     AddBook();     break;
    case Command.Remove:  RemoveBook();  break;
    case Command.Search:  SearchBook();  break;
    case Command.Borrow:  BorrowBook();  break;
    case Command.CheckIn: CheckInBook(); break;
    case Command.Exit:
        Console.WriteLine("Exiting Library Manager...");
        return;
    default:
        Console.WriteLine("Invalid input. Please enter a valid command.");
        break;
}
```

### Step 5 — Test the new features

Run the program and exercise every new path.

- Run the program and test the search and borrowing-limit functionalities with different inputs.
- When completed, **save your code** — you will use it to complete the final project in this course.

```bash
dotnet run
```

---

### Full reference solution

The complete program (the optimized base plus the three new features) for reference:

```csharp
using System.Globalization;

class Book(string title)
{
    public string Title { get; } = title;
    public bool IsBorrowed { get; set; } = false;
    public override string ToString() => $"{Title}" + (IsBorrowed ? " [Borrowed]" : "");
}

enum Command
{
    Add,
    Remove,
    Search,
    Borrow,
    CheckIn,
    Exit,
    Invalid
}

class LibraryManager
{
    private static int MaxBooks;
    private const int BorrowLimit = 3;
    private static int BorrowedCount = 0;
    private static List<Book?> books = default!;

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
                case Command.Search:
                    SearchBook();
                    break;
                case Command.Borrow:
                    BorrowBook();
                    break;
                case Command.CheckIn:
                    CheckInBook();
                    break;
                case Command.Exit:
                    Console.WriteLine("Exiting Library Manager...");
                    return;
                default:
                    Console.WriteLine("Invalid input. Please enter a valid command.");
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
        "search" => Command.Search,
        "borrow" => Command.Borrow,
        "checkin" => Command.CheckIn,
        "exit" => Command.Exit,
        _ => Command.Invalid
    };

    private static void PrintMenu()
    {
        Console.WriteLine("\nLibrary Manager");
        Console.WriteLine($"Books stored: {books.Count(b => b != null)} / {MaxBooks}");
        Console.WriteLine($"Books borrowed: {BorrowedCount} / {BorrowLimit}");
        Console.Write("Choose an action (add / remove / search / borrow / checkin / exit): ");
    }

    private static void AddBook()
    {
        if (!books.Any(b => b == null))
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
        if (books.All(b => b == null))
        {
            Console.WriteLine("The library is empty. No books to remove.");
            return;
        }
        string? title = GetValidatedInput("Enter the book title to remove: ");
        if (title == null) return;
        int index = books.FindIndex(b => b != null && b!.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (index >= 0)
        {
            if (books[index]!.IsBorrowed)
            {
                Console.WriteLine("Cannot remove a book that is currently borrowed.");
                return;
            }
            books[index] = null;
            Console.WriteLine($"Book \"{title}\" removed from the library.");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
    }

    private static void SearchBook()
    {
        string? title = GetValidatedInput("Enter the book title to search: ");
        if (title == null) return;
        var book = books.FirstOrDefault(b => b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book != null)
        {
            Console.WriteLine($"Book found: {book}");
        }
        else
        {
            Console.WriteLine("Book not found in the library.");
        }
    }

    private static void BorrowBook()
    {
        if (BorrowedCount >= BorrowLimit)
        {
            Console.WriteLine($"You cannot borrow more than {BorrowLimit} books at a time.");
            return;
        }
        string? title = GetValidatedInput("Enter the book title to borrow: ");
        if (title == null) return;
        var book = books.FirstOrDefault(b => b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book == null)
        {
            Console.WriteLine("Book not found.");
        }
        else if (book.IsBorrowed)
        {
            Console.WriteLine("That book is already borrowed.");
        }
        else
        {
            book.IsBorrowed = true;
            BorrowedCount++;
            Console.WriteLine($"You have borrowed \"{book.Title}\".");
        }
    }

    private static void CheckInBook()
    {
        string? title = GetValidatedInput("Enter the book title to check in: ");
        if (title == null) return;
        var book = books.FirstOrDefault(b => b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book == null)
        {
            Console.WriteLine("Book not found.");
        }
        else if (!book.IsBorrowed)
        {
            Console.WriteLine("That book is not currently borrowed.");
        }
        else
        {
            book.IsBorrowed = false;
            BorrowedCount--;
            Console.WriteLine($"You have checked in \"{book.Title}\".");
        }
    }

    private static void DisplayBooks()
    {
        Console.WriteLine("\nCurrent books in the library:");
        var nonEmptyBooks = books.Where(b => b != null).Select(b => b!.ToString()).ToList();
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
        return books.Any(b => b != null && b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
}
```

---

## ▶️ Expected result

The menu now offers `search`, `borrow`, and `checkin`. Searching reports whether a title is in
the collection; borrowing succeeds up to **3** books and is refused on the fourth; checking in
clears the borrowed flag and frees a borrow slot. The running `Books borrowed: N / 3` counter
in the menu stays accurate as you borrow and return.

---

## ☑️ Definition of done

- [ ] `SearchBook()` finds a title (case-insensitive) and reports found / not found
- [ ] `BorrowBook()` marks a book borrowed and **rejects a 4th** borrow with the limit message
- [ ] `CheckInBook()` clears `IsBorrowed`, decrements the count, and warns if the book wasn't borrowed
- [ ] `Command` enum, `ParseCommand`, and the `Main` `switch` route `search` / `borrow` / `checkin`
- [ ] `dotnet run` builds and the new features work across different inputs
- [ ] Code is **saved** for use as the course final project

---

## 🔑 Key concepts

- **Extend, don't restart** — new behavior is added as small, single-purpose methods
  (`SearchBook`, `BorrowBook`, `CheckInBook`) on the existing app, reusing its `Book` model
  and validation helpers.
- **Copilot as a pair programmer** — describe each challenge in plain language and let Copilot
  draft the method, then review and adapt the suggestion to fit the existing structure.
- **Enum-driven dispatch** — modeling actions as a `Command` enum keeps the `Main` loop a clean
  `switch`, so adding a feature means one enum value plus one method.
- **Stateful rules** — the borrow limit relies on a shared `BorrowedCount` that `Borrow` and
  `CheckIn` keep in sync, demonstrating how a guard (`>= BorrowLimit`) enforces a business rule.
- **Case-insensitive lookups** — `StringComparison.OrdinalIgnoreCase` makes search, borrow, and
  check-in forgiving of how the user types a title.
