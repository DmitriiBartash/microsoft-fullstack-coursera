# Writing Simple Programs in C#

**Course 2 — Introduction to Programming With C#** · Module 2 · Lesson 2 · `You Try It!`

> Write four small C# console programs that practise the building blocks of the
> language: a `Calculator` class with an `Add` method, a `Main` method that runs it,
> a `for` loop that prints numbers, and a routine that reads and greets the user.
> Then combine them into one menu-driven console app you can run end-to-end.

---

## 🎯 Objective

Practise the fundamentals of C# — **classes**, **methods**, **variables**, **loops**, and
**console input/output** — by writing four short programs and then assembling them into a
single runnable application.

---

## 🗂️ What you will build

A console project named **`SimplePrograms`** that grows across four exercises:

| Class           | Responsibility                                                       |
| --------------- | -------------------------------------------------------------------- |
| `Calculator`    | Add two integers and return the result (`Add`)                       |
| `NumberDisplay` | Print the numbers 1 to 10 with a `for` loop (`DisplayNumbers`)       |
| `UserInput`     | Prompt for a name and print a greeting (`GreetUser`)                  |
| `SimplePrograms`| `Main` — wire everything together behind a simple menu               |

**Flow:** `Main → GreetUser() → menu → Add() / DisplayNumbers() → Console output`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- The console application you created at the start of the course (you will paste each
  snippet into its `Program.cs`)

---

## 🛠️ Steps

### Step 1 — Creating a simple calculator

Create a simple calculator that performs basic arithmetic. Define a class called
`Calculator` and write a method called `Add` that takes two numbers and returns their sum.

```csharp
class Calculator
{
    // Adds two integers and returns the result
    public static int Add(int a, int b)
    {
        return a + b;
    }
}
```

- The `Add` method is `static`, so you can call it as `Calculator.Add(...)` without creating
  an instance.
- It takes two `int` parameters and returns an `int`.

### Step 2 — Executing the calculator program

Write a `Main` method to run the calculator. Assign values to two numbers, call `Add`, and
print the result with `Console.WriteLine`.

```csharp
class Calculator
{
    public static int Add(int a, int b)
    {
        return a + b;
    }

    static void Main()
    {
        int number1 = 8;
        int number2 = 5;

        int result = Add(number1, number2);
        Console.WriteLine("The sum is: " + result);
    }
}
```

> To check your answer, use the Visual Studio Code console application you created at the
> start of the course. Paste your code into the `Program.cs` file in that application (be
> sure to delete any existing code first). If you receive an error when you run the code, go
> to the reading on the next page to compare your code to the correct answer.

### Step 3 — Creating a loop to display numbers

Write a program that uses a loop to display the numbers from 1 to 10. Loops let you repeat an
action multiple times. Define a class called `NumberDisplay` with a method `DisplayNumbers`
that uses a `for` loop, and call it from `Main`.

```csharp
class NumberDisplay
{
    // Prints the numbers 1 through 10
    public static void DisplayNumbers()
    {
        Console.WriteLine("Numbers from 1 to 10:");
        for (int i = 1; i <= 10; i++)
        {
            Console.WriteLine(i);
        }
    }

    static void Main()
    {
        DisplayNumbers();
    }
}
```

- `i` starts at `1`, the loop continues while `i <= 10`, and `i++` increases it by one each
  pass — so the body runs exactly ten times.

### Step 4 — Handling user input

Write a program that interacts with the user: prompt for their name and greet them. Define a
class called `UserInput` with a method `GreetUser` that reads input with `Console.ReadLine`
and prints a greeting.

```csharp
class UserInput
{
    // Asks for the user's name and greets them
    public static void GreetUser()
    {
        Console.Write("Enter your name: ");
        string? name = Console.ReadLine();
        Console.WriteLine("Hello, " + name + "!");
    }

    static void Main()
    {
        GreetUser();
    }
}
```

- `Console.Write` keeps the cursor on the same line so the user types next to the prompt.
- `Console.ReadLine` returns the text the user typed (declared `string?` because it can be
  `null` if the input stream ends).

### Step 5 — Putting it all together

Combine the three classes into one program with a small menu. The full listing below brings
the calculator, the number loop, and the greeting together, and adds an `InputHelper` class
to read integers safely and pause between actions.

```csharp
namespace SimplePrograms
{
    class Calculator
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }

    class InputHelper
    {
        public static bool TryReadInteger(string prompt, out int number)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            return int.TryParse(input, out number);
        }

        public static void WaitForKey()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }

    class NumberDisplay
    {
        public static void DisplayNumbers()
        {
            Console.WriteLine("Numbers from 1 to 10:");
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine(i);
            }
        }
    }

    class UserInput
    {
        public static void GreetUser()
        {
            Console.Write("Enter your name: ");
            string? name = Console.ReadLine();
            Console.WriteLine("Hello, " + name + "!");
        }
    }

    class SimplePrograms
    {
        static void Main()
        {
            Console.WriteLine("--- Welcome to Simple Programs ---");
            UserInput.GreetUser();
            while (true)
            {
                Console.WriteLine("\n--- Main Menu ---");
                Console.WriteLine("1. Add two numbers");
                Console.WriteLine("2. Display numbers from 1 to 10");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();
                if (choice == "1")
                {
                    if (!InputHelper.TryReadInteger("Enter the first number: ", out int number1))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }
                    if (!InputHelper.TryReadInteger("Enter the second number: ", out int number2))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }
                    int result = Calculator.Add(number1, number2);
                    Console.WriteLine("The sum is: " + result);
                    InputHelper.WaitForKey();
                }
                else if (choice == "2")
                {
                    NumberDisplay.DisplayNumbers();
                    InputHelper.WaitForKey();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose 1, 2 or 3.");
                }
            }
        }
    }
}
```

> Paste this into `Program.cs` (clearing any existing code first) and run it with
> `dotnet run`.

---

## ▶️ Expected result

The program greets you by name, then shows a menu. Choosing **1** adds two numbers you type,
**2** prints the numbers 1 to 10, and **3** exits:

```text
--- Welcome to Simple Programs ---
Enter your name: Ada
Hello, Ada!

--- Main Menu ---
1. Add two numbers
2. Display numbers from 1 to 10
3. Exit
Choose an option: 1
Enter the first number: 8
Enter the second number: 5
The sum is: 13
Press Enter to continue...
```

---

## ☑️ Definition of done

- [ ] `Calculator.Add` returns the sum of its two `int` arguments
- [ ] A `Main` method assigns two numbers, calls `Add`, and prints the result
- [ ] `NumberDisplay.DisplayNumbers` uses a `for` loop to print 1 through 10
- [ ] `UserInput.GreetUser` reads a name with `Console.ReadLine` and prints a greeting
- [ ] The combined `SimplePrograms` app runs a menu loop and `dotnet run` produces the output above

---

## 🔑 Key concepts

- **Classes and methods** — group related behaviour into a class (`Calculator`,
  `NumberDisplay`, `UserInput`) and expose it through named methods that take parameters and
  return values.
- **`static` members** — `static` methods belong to the class itself, so they can be called
  as `ClassName.Method()` without first creating an object.
- **The `for` loop** — a counter (`int i = 1`), a condition (`i <= 10`), and a step (`i++`)
  control exactly how many times the loop body runs.
- **Console I/O** — `Console.Write`/`WriteLine` send output, `Console.ReadLine` reads a line
  of input; string concatenation with `+` builds the messages you display.
- **Validating input** — `int.TryParse` safely turns user text into a number and tells you
  whether it succeeded, so a bad entry never crashes the program.
