# Using Parameters in Methods

**Course 1 — Foundations of Coding Full-Stack** · Module 5 · Lesson 4 · `You Try It!`

> Build a small C# console app that solves **two calculation problems** using
> methods with **parameters**. The point of the lab is *reuse*: each calculation
> lives in its own focused method (`VolumeOfRectangle`, `CalculateAverage`), and a
> shared `ReadAndValidate` helper handles input — so `Main` just wires them together.

---

## 🎯 Objective

Use methods and parameters to create **reusable code blocks** in C# that perform basic
calculations — the volume of a rectangular box and the average of three numbers — while
keeping input reading and validation in one shared, reusable place.

---

## 🗂️ What you'll build

A single console project named **`ParamsInMethods`** with one class that exposes four
methods:

| Method                                                   | Responsibility                                              |
| -------------------------------------------------------- | ----------------------------------------------------------- |
| `Main()`                                                 | Orchestrate: read inputs, call the calculations, print results |
| `ReadAndValidate(string prompt)`                         | Prompt, read a line, and return a validated positive `int`  |
| `VolumeOfRectangle(int length, int width, int height)`   | Return `length * width * height`                            |
| `CalculateAverage(int num1, int num2, int num3)`         | Return the integer average of three numbers                |

**Flow:** `Main → ReadAndValidate(prompt) → int → VolumeOfRectangle / CalculateAverage → result`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (or any C# editor)
- Comfortable running a console app with `dotnet run`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold the console app and move into it.

```bash
dotnet new console -n ParamsInMethods
cd ParamsInMethods
```

- Open `Program.cs` and **clear any existing content** — you'll replace it with the class below.

### Step 2 — Problem 1: volume of a rectangular box

Write a method that calculates the volume of a rectangular box. It accepts three integer
parameters — `length`, `width`, and `height` — and returns:

```text
Volume = length * width * height
```

```csharp
public static int VolumeOfRectangle(int length, int width, int height)
{
    return length * width * height;
}
```

### Step 3 — Problem 2: average of three numbers

Write a method that calculates the average of three integers. It accepts three parameters —
`num1`, `num2`, and `num3` — and returns the average as an **integer** (integer division).

```csharp
public static int CalculateAverage(int num1, int num2, int num3)
{
    return (num1 + num2 + num3) / 3;
}
```

### Step 4 — Add a reusable input helper

Rather than repeat reading-and-validating logic for every value, factor it into one method.
`ReadAndValidate` loops until it gets a **valid integer greater than 0**, rejecting empty
input and non-numeric input.

```csharp
public static int ReadAndValidate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Input is empty or whitespace.");
            continue;
        }
        if (!int.TryParse(input, out int number) || number <= 0)
        {
            Console.WriteLine("Input must be a valid integer greater than 0.");
            continue;
        }
        return number;
    }
}
```

### Step 5 — Wire everything together in `Main`

In `Main`, read each value through `ReadAndValidate`, pass them as **arguments** to the
calculation methods, and print the results. Here is the complete `Program.cs`:

```csharp
namespace ParamsInMethods
{
    public class ParamsInMethods
    {
        public static void Main()
        {
            // Problem 1
            int length = ReadAndValidate("Enter the length: ");
            int width = ReadAndValidate("Enter the width: ");
            int height = ReadAndValidate("Enter the height: ");
            int volume = VolumeOfRectangle(length, width, height);
            Console.WriteLine($"The volume of the rectangular box is: {volume}");

            // Problem 2
            int num1 = ReadAndValidate("Enter the first number: ");
            int num2 = ReadAndValidate("Enter the second number: ");
            int num3 = ReadAndValidate("Enter the third number: ");
            int average = CalculateAverage(num1, num2, num3);
            Console.WriteLine($"The average of the three numbers is: {average}");
        }

        public static int ReadAndValidate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input is empty or whitespace.");
                    continue;
                }
                if (!int.TryParse(input, out int number) || number <= 0)
                {
                    Console.WriteLine("Input must be a valid integer greater than 0.");
                    continue;
                }
                return number;
            }
        }

        public static int VolumeOfRectangle(int length, int width, int height)
        {
            return length * width * height;
        }

        public static int CalculateAverage(int num1, int num2, int num3)
        {
            return (num1 + num2 + num3) / 3;
        }
    }
}
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The program prompts for length, width, and height, then prints the box volume. It then
prompts for three numbers and prints their integer average. For example, entering
`2`, `3`, `4` for the box and `10`, `20`, `30` for the numbers:

```text
The volume of the rectangular box is: 24
The average of the three numbers is: 20
```

---

## ☑️ Definition of done

- [ ] `ParamsInMethods` console project created and `Program.cs` cleared
- [ ] `VolumeOfRectangle(length, width, height)` returns the product of the three dimensions
- [ ] `CalculateAverage(num1, num2, num3)` returns the integer average of three numbers
- [ ] `ReadAndValidate(prompt)` rejects empty/whitespace and non-positive input, looping until valid
- [ ] `Main` reads inputs, calls both methods with arguments, and prints both results
- [ ] `dotnet run` produces the volume and the average

---

## 🔑 Key concepts

- **Parameters carry data into methods** — `length`, `width`, `height`, `num1`–`num3` are
  *parameters* in the signature; the values you pass from `Main` are *arguments*.
- **Return values flow data back out** — each method `return`s an `int`, so callers can store
  the result (`volume`, `average`) and reuse it.
- **Reuse over repetition (DRY)** — extracting `ReadAndValidate` means input validation is
  written once and called six times, instead of copy-pasted per value.
- **Integer division truncates** — `(num1 + num2 + num3) / 3` discards the fractional part,
  which is why the average is returned as an `int` (e.g. `10/3` → `3`).
- **`int.TryParse` is safe parsing** — it returns `false` instead of throwing on bad input,
  making it the idiomatic way to validate user-entered numbers.
