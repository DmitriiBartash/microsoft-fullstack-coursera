# Developing Programs with Functions and Methods

**Course 1 — Foundations of Coding Full-Stack** · Module 5 · Lesson 5 · `You Try It!`

> Build a single C# console program that grows step by step into a small interactive
> app. Along the way you practice the core building blocks of **methods**: defining and
> calling them, passing **parameters**, returning **values**, and using a `bool` result
> to drive **conditional logic** — finishing with a real-world age-validation check.

---

## 🎯 Objective

By the end of this activity you will be able to develop simple programs using
**functions and methods in C#** to solve real-world problems. You will practice
**defining and calling** methods, **passing parameters**, and **using return values**,
then combine them with input handling and conditional logic in a single runnable app.

---

## 🗂️ What you will build

One console project, `DevelopWithFunctAndMeth`, whose `Main` method orchestrates a set
of small, single-purpose methods built up across five steps:

| Method                  | Signature                          | Responsibility                                  |
| ----------------------- | ---------------------------------- | ----------------------------------------------- |
| `DisplayWelcomeMessage` | `void DisplayWelcomeMessage()`     | Print a fixed welcome banner                     |
| `GreetUser`             | `void GreetUser(string name)`      | Greet a user by name (takes a **parameter**)     |
| `CalculateSum`          | `int CalculateSum(int x, int y)`   | Add two numbers and **return** the sum           |
| `IsPositive`            | `bool IsPositive(int number)`      | **Return** whether a number is greater than zero |
| `IsOldEnoughToDrive`    | `bool IsOldEnoughToDrive(int age)` | **Return** whether `age >= 18`                   |
| `ReadInteger`           | `int ReadInteger(string prompt)`   | Read and validate integer input (helper)         |
| `GetUserName`           | `string GetUserName()`             | Read and validate a non-empty name (helper)      |

**Flow:** `Main  →  welcome  →  greet  →  sum  →  positivity check  →  drive-age check`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor such as Visual Studio Code or Visual Studio
- Comfort running a console app from the terminal with `dotnet run`

---

## 🛠️ Steps

### Step 1 — Defining and calling a simple method

Create a method that prints a welcome message to the console. This demonstrates the
basic structure and use of a method in C#.

- Define a method called `DisplayWelcomeMessage` that prints `"Welcome to the Program!"`.
- Call the method from `Main` to execute it.

```csharp
public static void DisplayWelcomeMessage()
{
    Console.WriteLine("Welcome to the Program!");
}

// In Main:
DisplayWelcomeMessage();
```

### Step 2 — Creating a method with parameters

Create a method that takes a **parameter** to produce personalized output. It greets a
user by name.

- Define a method called `GreetUser` that takes a `string` parameter `name` and prints `"Hello, [name]!"`.
- Call the method from `Main`, passing a name as an argument.

```csharp
public static void GreetUser(string name)
{
    Console.WriteLine($"Hello, {name}!");
}

// In Main:
string name = GetUserName();   // see helper below
GreetUser(name);
```

### Step 3 — Using methods with return values

Create a method that **returns a value** — it adds two numbers together and returns the
sum.

- Define a method called `CalculateSum` that takes two `int` parameters and returns their sum.
- Store the result of the call in a variable and print it.

```csharp
public static int CalculateSum(int x, int y) => x + y;

// In Main:
int num1 = ReadInteger("Enter the first number: ");
int num2 = ReadInteger("Enter the second number: ");
int result = CalculateSum(num1, num2);
Console.WriteLine($"The sum of {num1} and {num2} is {result}.");
```

### Step 4 — Combining methods and conditional logic

Create a method that **returns a boolean** based on a condition, then branch on it. This
method checks whether a number is positive.

- Define a method called `IsPositive` that takes an `int` and returns `true` if the number is greater than zero, otherwise `false`.
- Use an `if`/`else` statement in `Main` to check the result and print whether the number is positive.

```csharp
public static bool IsPositive(int number)
{
    return number > 0;
}

// In Main:
int numberToCheck = ReadInteger("Enter a number to check if it is positive: ");
if (IsPositive(numberToCheck))
{
    Console.WriteLine($"{numberToCheck} is a positive number.");
}
else
{
    Console.WriteLine($"{numberToCheck} is not a positive number.");
}
```

### Step 5 — Practical application: user age validation

Put it all together to validate user input. The program asks for the user's age and
checks whether they are old enough to drive.

- Define a method called `IsOldEnoughToDrive` that takes an `int age` and returns `true` if the age is 18 or older.
- In `Main`, prompt the user to enter their age.
- Convert the input to an integer (here via the validating `ReadInteger` helper, which uses `int.TryParse`).
- Call the method and print whether the user is old enough to drive based on its return value.

```csharp
public static bool IsOldEnoughToDrive(int age)
{
    return age >= 18;
}

// In Main:
int age = ReadInteger("Please enter your age: ");
if (IsOldEnoughToDrive(age))
{
    Console.WriteLine("You are old enough to drive.");
}
else
{
    Console.WriteLine("You are not old enough to drive.");
}
```

### Full program

Combine every step into one file. Two small helpers — `GetUserName` and `ReadInteger` —
keep the input reading and validation in their own methods, so `Main` stays readable.

```csharp
namespace DevelopWithFunctAndMeth
{
    public class DevelopWithFunctAndMeth
    {
        public static void Main()
        {
            // Step 1: Welcome message
            DisplayWelcomeMessage();

            // Step 2: Greet user
            string name = GetUserName();
            GreetUser(name);

            // Step 3: Sum calculation
            int num1 = ReadInteger("Enter the first number: ");
            int num2 = ReadInteger("Enter the second number: ");
            int result = CalculateSum(num1, num2);
            Console.WriteLine($"The sum of {num1} and {num2} is {result}.");

            // Step 4: Check if a user-provided number is positive
            int numberToCheck = ReadInteger("Enter a number to check if it is positive: ");
            if (IsPositive(numberToCheck))
            {
                Console.WriteLine($"{numberToCheck} is a positive number.");
            }
            else
            {
                Console.WriteLine($"{numberToCheck} is not a positive number.");
            }

            // Step 5: Validate user age
            int age = ReadInteger("Please enter your age: ");
            if (IsOldEnoughToDrive(age))
            {
                Console.WriteLine("You are old enough to drive.");
            }
            else
            {
                Console.WriteLine("You are not old enough to drive.");
            }
        }

        public static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to the Program!");
        }

        public static string GetUserName()
        {
            Console.Write("Please enter your name: ");
            string? input = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Try again.");
                Console.Write("Please enter your name: ");
                input = Console.ReadLine();
            }
            return input;
        }

        public static void GreetUser(string name)
        {
            Console.WriteLine($"Hello, {name}!");
        }

        public static int ReadInteger(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            int number;
            while (!int.TryParse(input, out number))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return number;
        }

        public static int CalculateSum(int x, int y) => x + y;

        public static bool IsPositive(int number)
        {
            return number > 0;
        }

        public static bool IsOldEnoughToDrive(int age)
        {
            return age >= 18;
        }
    }
}
```

Run it from the project folder:

```bash
dotnet run
```

---

## ▶️ Expected result

The program runs top to bottom, exercising every method in turn. A sample session:

```text
Welcome to the Program!
Please enter your name: Ada
Hello, Ada!
Enter the first number: 7
Enter the second number: 5
The sum of 7 and 5 is 12.
Enter a number to check if it is positive: -3
-3 is not a positive number.
Please enter your age: 20
You are old enough to drive.
```

Invalid integer input is re-prompted (`int.TryParse`) and an empty name is rejected, so
the program never crashes on bad input.

---

## ☑️ Definition of done

- [ ] `DisplayWelcomeMessage` is defined and called from `Main`, printing the welcome banner
- [ ] `GreetUser(string name)` greets the user by name using its parameter
- [ ] `CalculateSum(int x, int y)` returns the sum, and the result is stored and printed
- [ ] `IsPositive(int number)` returns a `bool` and drives an `if`/`else` branch in `Main`
- [ ] `IsOldEnoughToDrive(int age)` returns `true` for ages `>= 18` and prints the right message
- [ ] `dotnet run` completes the full session and handles invalid input without crashing

---

## 🔑 Key concepts

- **Methods package behavior** — each task (greet, sum, check) lives in its own named
  method, so `Main` reads as a high-level script and each piece can be reused.
- **Parameters pass data in** — `GreetUser(string name)` and `CalculateSum(int x, int y)`
  receive arguments from the caller instead of relying on globals.
- **Return values pass data out** — a method's result (`int`, `bool`, `string`) flows back
  to the caller, where it can be stored in a variable or used directly.
- **Booleans drive control flow** — methods like `IsPositive` and `IsOldEnoughToDrive`
  return `bool`, which plugs straight into `if`/`else` for clean decision logic.
- **Validate input at the edge** — `int.TryParse` and `string.IsNullOrWhiteSpace` loops
  keep bad input out, so the rest of the program can trust its values.
