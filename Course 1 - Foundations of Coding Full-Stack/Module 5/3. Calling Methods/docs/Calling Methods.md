# Calling Methods

**Course 1 — Foundations of Coding Full-Stack** · Module 5 · Lesson 3 · `You Try It!`

> Build one small C# console program that exercises every part of **calling methods**:
> defining a method, calling it, passing **parameters**, and using **return values** to
> drive the program. `Main` reads input from the console and hands it off to five small,
> single-purpose methods.

---

## 🎯 Objective

By the end of this activity, you will be able to **call methods in C#** within a complete
program. You will gain hands-on experience defining methods, calling methods with
**parameters**, and using method **return values** to solve practical problems.

---

## 🗂️ What you will build

A single console program in a class named **`CallingMethods`**, whose `Main` method calls
five helper methods in turn.

| Method                          | Parameters     | Returns  | Purpose                                          |
| ------------------------------- | -------------- | -------- | ------------------------------------------------ |
| `DisplayWelcomeMessage()`       | none           | `void`   | Print a fixed welcome message                    |
| `DisplayWelcomeMessageWithName(string name)` | `name` | `void` | Greet the user by name                       |
| `CalculateSum(int x, int y)`    | `x`, `y`       | `int`    | Add two numbers and return the result            |
| `IsPositive(int number)`        | `number`       | `bool`   | Report whether a number is `>= 0`                |
| `IsOldEnoughToDrive(int age)`   | `age`          | `bool`   | Report whether an age is `>= 18`                 |

**Flow:** `Main → reads input → calls method → uses return value → prints result`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor such as Visual Studio Code
- Basic C# familiarity: `Console.WriteLine`, variables, and `if`/`else`

---

## 🛠️ Steps

### Step 1 — Scaffold the project

Create a console app, move into it, and open it in your editor.

```bash
dotnet new console -n CallingMethods
cd CallingMethods
```

- Open `Program.cs` and **clear any existing content** so you can build it up step by step.

### Step 2 — Define and call a simple method

Define a method named `DisplayWelcomeMessage` that prints `"Welcome to our Program!"`, then
call it from `Main`. A `void` method does work but returns no value.

```csharp
static void DisplayWelcomeMessage()
{
    Console.WriteLine("Welcome to our Program!");
}
```

Call it as the first line of `Main`:

```csharp
DisplayWelcomeMessage();
```

### Step 3 — Pass a parameter

Read the user's name, then pass it to a method that greets them. This shows how a **parameter**
flows a value *into* a method.

```csharp
Console.Write("Input your name: ");
string? input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input))
{
    Console.WriteLine("Input cannot be empty. Please try again.");
    return;
}
DisplayWelcomeMessageWithName(input);
```

```csharp
static void DisplayWelcomeMessageWithName(string name)
{
    Console.WriteLine("Hello " + name + "!");
}
```

### Step 4 — Use a return value

Read two numbers, call `CalculateSum`, and capture the **value it returns** in a variable so the
caller can print it.

```csharp
Console.WriteLine("Let's calculate the sum of two values.");
Console.Write("Input a: ");
input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int a))
{
    Console.WriteLine("Invalid input for 'a'. Please enter a valid number.");
    return;
}
Console.Write("Input b: ");
input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int b))
{
    Console.WriteLine("Invalid input for 'b'. Please enter a valid number.");
    return;
}
int result = CalculateSum(a, b);
Console.WriteLine($"The sum of {a} and {b} is {result}.");
```

```csharp
static int CalculateSum(int x, int y)
{
    return x + y;
}
```

### Step 5 — Branch on a `bool` return value

Read a number, call `IsPositive`, and use the returned `bool` to choose what to print.

```csharp
Console.WriteLine("Let's check if a number is positive.");
Console.Write("Input a number: ");
input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int number))
{
    Console.WriteLine("Invalid input. Please enter a valid number.");
    return;
}
bool isPositive = IsPositive(number);
if (isPositive)
{
    Console.WriteLine("The number is positive.");
}
else
{
    Console.WriteLine("The number is negative.");
}
```

```csharp
static bool IsPositive(int number)
{
    return number >= 0;
}
```

### Step 6 — Reuse the pattern: another `bool` method

Apply the same call-and-branch pattern to an age check with `IsOldEnoughToDrive`.

```csharp
Console.WriteLine("Let's check if you're old enough to drive.");
Console.Write("Input your age: ");
input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int age))
{
    Console.WriteLine("Invalid age input. Please enter a valid number.");
    return;
}
bool canDrive = IsOldEnoughToDrive(age);
if (canDrive)
{
    Console.WriteLine("You are old enough to drive.");
}
else
{
    Console.WriteLine("Sorry, you are not old enough to drive.");
}
```

```csharp
static bool IsOldEnoughToDrive(int age)
{
    return age >= 18;
}
```

### Step 7 — Full program

Put it all together. This is the complete `CallingMethods` class.

```csharp
class CallingMethods
{
    static void Main()
    {
        // Step 1
        DisplayWelcomeMessage();

        // Step 2
        Console.Write("Input your name: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Input cannot be empty. Please try again.");
            return;
        }
        DisplayWelcomeMessageWithName(input);

        // Step 3
        Console.WriteLine("Let's calculate the sum of two values.");
        Console.Write("Input a: ");
        input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int a))
        {
            Console.WriteLine("Invalid input for 'a'. Please enter a valid number.");
            return;
        }
        Console.Write("Input b: ");
        input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int b))
        {
            Console.WriteLine("Invalid input for 'b'. Please enter a valid number.");
            return;
        }
        int result = CalculateSum(a, b);
        Console.WriteLine($"The sum of {a} and {b} is {result}.");

        // Step 4
        Console.WriteLine("Let's check if a number is positive.");
        Console.Write("Input a number: ");
        input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int number))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            return;
        }
        bool isPositive = IsPositive(number);
        if (isPositive)
        {
            Console.WriteLine("The number is positive.");
        }
        else
        {
            Console.WriteLine("The number is negative.");
        }

        // Step 5
        Console.WriteLine("Let's check if you're old enough to drive.");
        Console.Write("Input your age: ");
        input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int age))
        {
            Console.WriteLine("Invalid age input. Please enter a valid number.");
            return;
        }
        bool canDrive = IsOldEnoughToDrive(age);
        if (canDrive)
        {
            Console.WriteLine("You are old enough to drive.");
        }
        else
        {
            Console.WriteLine("Sorry, you are not old enough to drive.");
        }
    }

    static void DisplayWelcomeMessage()
    {
        Console.WriteLine("Welcome to our Program!");
    }

    static void DisplayWelcomeMessageWithName(string name)
    {
        Console.WriteLine("Hello " + name + "!");
    }

    static int CalculateSum(int x, int y)
    {
        return x + y;
    }

    static bool IsPositive(int number)
    {
        return number >= 0;
    }

    static bool IsOldEnoughToDrive(int age)
    {
        return age >= 18;
    }
}
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The program greets you, then walks through each check using your console input. A sample run:

```text
Welcome to our Program!
Input your name: Ada
Hello Ada!
Let's calculate the sum of two values.
Input a: 7
Input b: 5
The sum of 7 and 5 is 12.
Let's check if a number is positive.
Input a number: -3
The number is negative.
Let's check if you're old enough to drive.
Input your age: 20
You are old enough to drive.
```

---

## ☑️ Definition of done

- [ ] `CallingMethods` console project created and `Program.cs` cleared
- [ ] `DisplayWelcomeMessage()` defined and called from `Main`
- [ ] `DisplayWelcomeMessageWithName(name)` greets the user using a **parameter**
- [ ] `CalculateSum(x, y)` returns an `int` that the caller captures and prints
- [ ] `IsPositive(number)` and `IsOldEnoughToDrive(age)` each return a `bool` used in an `if`/`else`
- [ ] Invalid or empty input is handled with a clear message and an early `return`
- [ ] `dotnet run` walks through all five checks end-to-end

---

## 🔑 Key concepts

- **Defining vs. calling** — a method is declared once (its body) but can be *called* many times by name; `Main` is itself a method the runtime calls for you.
- **Parameters pass data in** — `DisplayWelcomeMessageWithName(name)` and `CalculateSum(x, y)` receive caller values through their parameter list (arguments matched by position).
- **Return values pass data out** — `CalculateSum` returns an `int` and `IsPositive`/`IsOldEnoughToDrive` return a `bool`; the caller stores the result and acts on it.
- **`void` vs. value-returning** — `void` methods do work for their side effect (printing); value-returning methods compute an answer the caller consumes.
- **Validate before you call** — `int.TryParse` and `string.IsNullOrWhiteSpace` guard against bad input, returning early so methods only run on valid data (fail fast).
