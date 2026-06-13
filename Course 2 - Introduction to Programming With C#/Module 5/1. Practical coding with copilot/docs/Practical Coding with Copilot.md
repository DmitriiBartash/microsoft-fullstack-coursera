# Practical Coding with Copilot

**Course 2 — Introduction to Programming With C#** · Module 5 · Lesson 1 · `You Try It!`

> Build a small C# console **library management system** with the help of
> **Microsoft Copilot**. The app stores up to **five** book titles in `string`
> variables and lets a user **add**, **remove**, and **display** books from a
> menu loop. The goal is to practice prompting Copilot to generate idiomatic C#
> that uses string variables, conditional statements, and loops.

---

## 🎯 Objective

Create a simple library management system that stores up to **five** book titles
using `string` variables. With Copilot's help, the program lets users **add** a
new book, **remove** a book by title, and **display** the current list of books —
running in a continuous menu loop until the user chooses to exit.

---

## 🗂️ What you will build

A single-file C# console application in the `LibraryManagement` namespace, driven
by an interactive menu loop.

| Action      | What it does                                                              |
| ----------- | ------------------------------------------------------------------------- |
| `add`       | Prompt for a title and store it in the first empty slot (full → refuse)   |
| `remove`    | Prompt for a title and clear the matching slot (not found → report)       |
| `display`   | Print every non-empty slot (none → `(No books available)`)                |
| `exit`      | Break the loop and end the program                                        |
| *(other)*   | Reject invalid input and prompt again                                     |

**Flow:** `menu prompt → read action → add / remove / display / exit → repeat`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- An editor with **Microsoft Copilot** available (e.g. Visual Studio Code with the Copilot extension, or Visual Studio)
- Basic familiarity with C# `string` variables, `if`/`else if`, and `while` loops

---

## 🛠️ Steps

Create a C# console application that manages a small library of books. Drive the
implementation with Copilot prompts, then refine the result until it matches the
behavior below.

### Step 1 — Scaffold the console application

Create a new console project named `LibraryManagement` and open it.

```bash
dotnet new console -n LibraryManagement
cd LibraryManagement
```

### Step 2 — Create variables for books

Set up **five** `string` variables to store the book titles, each initialized to
an empty string so an empty slot is easy to detect.

```csharp
// Step 1: Create 5 string variables for book storage
string book1 = "";
string book2 = "";
string book3 = "";
string book4 = "";
string book5 = "";
```

### Step 3 — Add a book

Prompt the user for a title, check which book variable is empty, and store the
new book there. If **all** slots are full, inform the user that no more books can
be added.

```csharp
if (action == "add")
{
    // Check if there is any empty slot
    if (book1 != "" && book2 != "" && book3 != "" && book4 != "" && book5 != "")
    {
        Console.WriteLine("Library is full. Cannot add more books.");
        continue;
    }
    Console.Write("Enter the title of the book to add: ");
    string newBook = Console.ReadLine();
    if (book1 == "") book1 = newBook;
    else if (book2 == "") book2 = newBook;
    else if (book3 == "") book3 = newBook;
    else if (book4 == "") book4 = newBook;
    else if (book5 == "") book5 = newBook;
    Console.WriteLine($"Book \"{newBook}\" added.");
}
```

### Step 4 — Remove a book

Ask the user for the title to remove. If it exists in the collection, clear the
corresponding variable; otherwise report that the book was not found. Only allow
removing when there is at least one book in the library.

```csharp
else if (action == "remove")
{
    // Check if there's at least one book to remove
    if (book1 == "" && book2 == "" && book3 == "" && book4 == "" && book5 == "")
    {
        Console.WriteLine("Library is empty. Nothing to remove.");
        continue;
    }
    Console.Write("Enter the title of the book to remove: ");
    string removeBook = Console.ReadLine();
    if (book1 == removeBook) { book1 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
    else if (book2 == removeBook) { book2 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
    else if (book3 == removeBook) { book3 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
    else if (book4 == removeBook) { book4 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
    else if (book5 == removeBook) { book5 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
    else
    {
        Console.WriteLine("Book not found.");
    }
}
```

### Step 5 — Display the list of books

Print only the **non-empty** book slots. If every slot is empty, show a
`(No books available)` message instead.

```csharp
else if (action == "display")
{
    Console.WriteLine("\nBooks in Library:");
    if (book1 != "") Console.WriteLine("- " + book1);
    if (book2 != "") Console.WriteLine("- " + book2);
    if (book3 != "") Console.WriteLine("- " + book3);
    if (book4 != "") Console.WriteLine("- " + book4);
    if (book5 != "") Console.WriteLine("- " + book5);
    if (book1 == "" && book2 == "" && book3 == "" && book4 == "" && book5 == "")
        Console.WriteLine("(No books available)");
}
```

### Step 6 — Loop indefinitely and handle invalid input

Wrap everything in a `while (true)` loop that keeps prompting for an action.
Normalize the input with `Trim().ToLower()`. If the user types `exit`, break the
loop and end the program. If the action is none of `add` / `remove` / `display` /
`exit`, report it and prompt again.

```csharp
else if (action == "exit")
{
    Console.WriteLine("Exiting the Library Management System.");
    break;
}
else
{
    Console.WriteLine("Invalid action. Please type add, remove, display, or exit.");
}
```

### Step 7 — Assemble and run the full program

Put the pieces together into the complete program, then run it. When completed,
**save your code** — you will reuse it in the final project for this course.

```csharp
namespace LibraryManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            // Step 1: Create 5 string variables for book storage
            string book1 = "";
            string book2 = "";
            string book3 = "";
            string book4 = "";
            string book5 = "";

            while (true)
            {
                Console.WriteLine("\nChoose an action: add / remove / display / exit");
                string action = Console.ReadLine().Trim().ToLower();

                if (action == "add")
                {
                    // Check if there is any empty slot
                    if (book1 != "" && book2 != "" && book3 != "" && book4 != "" && book5 != "")
                    {
                        Console.WriteLine("Library is full. Cannot add more books.");
                        continue;
                    }
                    Console.Write("Enter the title of the book to add: ");
                    string newBook = Console.ReadLine();
                    if (book1 == "") book1 = newBook;
                    else if (book2 == "") book2 = newBook;
                    else if (book3 == "") book3 = newBook;
                    else if (book4 == "") book4 = newBook;
                    else if (book5 == "") book5 = newBook;
                    Console.WriteLine($"Book \"{newBook}\" added.");
                }
                else if (action == "remove")
                {
                    // Check if there's at least one book to remove
                    if (book1 == "" && book2 == "" && book3 == "" && book4 == "" && book5 == "")
                    {
                        Console.WriteLine("Library is empty. Nothing to remove.");
                        continue;
                    }
                    Console.Write("Enter the title of the book to remove: ");
                    string removeBook = Console.ReadLine();
                    if (book1 == removeBook) { book1 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book2 == removeBook) { book2 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book3 == removeBook) { book3 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book4 == removeBook) { book4 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book5 == removeBook) { book5 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else
                    {
                        Console.WriteLine("Book not found.");
                    }
                }
                else if (action == "display")
                {
                    Console.WriteLine("\nBooks in Library:");
                    if (book1 != "") Console.WriteLine("- " + book1);
                    if (book2 != "") Console.WriteLine("- " + book2);
                    if (book3 != "") Console.WriteLine("- " + book3);
                    if (book4 != "") Console.WriteLine("- " + book4);
                    if (book5 != "") Console.WriteLine("- " + book5);
                    if (book1 == "" && book2 == "" && book3 == "" && book4 == "" && book5 == "")
                        Console.WriteLine("(No books available)");
                }
                else if (action == "exit")
                {
                    Console.WriteLine("Exiting the Library Management System.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid action. Please type add, remove, display, or exit.");
                }
            }
        }
    }
}
```

```bash
dotnet run
```

---

## ▶️ Expected result

The program loops, prompting `Choose an action: add / remove / display / exit`.
Typing `add` stores a title in the first empty slot (or refuses once five are
full); `remove` clears a matching title (or reports "Book not found."); `display`
lists the current books (or `(No books available)`); `exit` prints
`Exiting the Library Management System.` and ends. Any other input prints
`Invalid action.` and prompts again.

---

## ☑️ Definition of done

- [ ] `LibraryManagement` console project created and runs with `dotnet run`
- [ ] Five `string` book variables hold up to five titles
- [ ] `add` fills the first empty slot and refuses when the library is full
- [ ] `remove` clears a matching title and reports when a title is not found
- [ ] `display` shows only non-empty slots, or `(No books available)` when empty
- [ ] `exit` breaks the loop; any invalid action is rejected and re-prompted
- [ ] Code saved for reuse in the course final project

---

## 🔑 Key concepts

- **Prompting Copilot effectively** — a clear problem statement plus explicit
  steps (variables, add, remove, display, loop, error handling) lets Copilot
  generate idiomatic C# you can refine, rather than guessing at requirements.
- **String variables as fixed storage** — five named `string` slots model a
  fixed-capacity collection; an empty string (`""`) marks a free slot.
- **Conditional flow with `if`/`else if`** — chained comparisons route the chosen
  action and find the first empty (add) or matching (remove) slot.
- **Menu loops with `while (true)`** — the loop keeps the app interactive;
  `break` exits on `exit` and `continue` skips a turn when an action can't proceed.
- **Defensive input handling** — `Trim().ToLower()` normalizes input, and full /
  empty / not-found checks keep the program robust against invalid actions.
