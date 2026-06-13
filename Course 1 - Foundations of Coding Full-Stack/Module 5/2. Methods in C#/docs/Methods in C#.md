# Methods in C#

**Course 1 — Foundations of Coding Full-Stack** · Module 5 · Lesson 2 · `Activity`

> Practice writing **methods** in C# by solving two small geometry problems. Each
> calculation lives in its own reusable method (`CalculateCircleArea`,
> `CalculateTrapezoidArea`), while `Main` handles input, calls the method, and
> prints the result — the core pattern of *parameters in, value out*.

---

## 🎯 Objective

Use methods in C#: their **definition**, **syntax**, and **use cases**. You will write
two value-returning methods that each take one or more parameters, then call them from
`Main` to compute and display the area of a circle and a trapezoid.

---

## 🗂️ What you'll build

A single .NET console project named **`MethodsDemo`** that solves two problems:

| Problem | Method                                                | Parameters                  | Returns        |
| ------- | ----------------------------------------------------- | --------------------------- | -------------- |
| 1       | `CalculateCircleArea(double radius)`                  | `radius`                    | `double` area  |
| 2       | `CalculateTrapezoidArea(double a, double b, double height)` | `a`, `b`, `height`    | `double` area  |

**Flow:** `Main → read input → TryParse → Calculate…Area(args) → area → print`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor such as Visual Studio Code or Visual Studio
- Basic familiarity with `Console.ReadLine`, `Console.WriteLine`, and `double.TryParse`

---

## 🛠️ Steps

### Step 1 — Create the console project

Scaffold a new console application and move into it.

```bash
dotnet new console -n MethodsDemo
cd MethodsDemo
```

Open `Program.cs` and clear any existing content so you can build it up from scratch.

### Step 2 — Problem 1: a method for circle area

**Problem statement:** Write a method to calculate the area of a circle. The method
accepts one input parameter — the **radius**. The program prompts the user for this
value, uses the method to compute the area, then displays the result.

Define a value-returning method that takes the radius and returns `π · r²`:

```csharp
// Method to calculate area of a circle
static double CalculateCircleArea(double radius)
{
    return Math.PI * radius * radius;
}
```

### Step 3 — Problem 2: a method for trapezoid area

**Problem statement:** Write a method to calculate the area of a trapezoid. The method
accepts three input parameters — the lengths of the two parallel sides (`a` and `b`)
and the **height**. The program prompts for these values, computes the area, and
displays the result.

**Formula:** the area of a trapezoid is `(a + b) / 2 * height`.

```csharp
// Method to calculate area of a trapezoid
static double CalculateTrapezoidArea(double a, double b, double height)
{
    return (a + b) / 2 * height;
}
```

### Step 4 — Wire it up in `Main`

In `Main`, prompt for each value, parse the input safely with `double.TryParse`, call
the matching method, and print the result formatted to two decimals (`:F2`). Here is the
complete program:

```csharp
class Program
{
    static void Main()
    {
        // ----- Problem 1: circle -----
        Console.Write("Enter the radius of the circle: ");
        string? input = Console.ReadLine();

        // Convert string to double
        if (double.TryParse(input, out double radius))
        {
            double area = CalculateCircleArea(radius);
            Console.WriteLine($"The area of the circle is: {area:F2}");
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
        }

        // ----- Problem 2: trapezoid -----
        Console.Write("Enter the length of side a: ");
        string? inputA = Console.ReadLine();
        Console.Write("Enter the length of side b: ");
        string? inputB = Console.ReadLine();
        Console.Write("Enter the height: ");
        string? inputHeight = Console.ReadLine();

        // Parse inputs to double
        if (double.TryParse(inputA, out double a) &&
            double.TryParse(inputB, out double b) &&
            double.TryParse(inputHeight, out double height))
        {
            double area = CalculateTrapezoidArea(a, b, height);
            Console.WriteLine($"The area of the trapezoid is: {area:F2}");
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter numeric values.");
        }
    }

    // Method to calculate area of a circle
    static double CalculateCircleArea(double radius)
    {
        return Math.PI * radius * radius;
    }

    // Method to calculate area of a trapezoid
    static double CalculateTrapezoidArea(double a, double b, double height)
    {
        return (a + b) / 2 * height;
    }
}
```

### Step 5 — Run it

```bash
dotnet run
```

Enter the prompted values when asked.

---

## ▶️ Expected result

With a radius of `5` and trapezoid sides `a = 4`, `b = 6`, `height = 3`, the program prints:

```text
Enter the radius of the circle: 5
The area of the circle is: 78.54
Enter the length of side a: 4
Enter the length of side b: 6
Enter the height: 3
The area of the trapezoid is: 15.00
```

Non-numeric input falls through to the friendly "Invalid input" message instead of crashing.

---

## ☑️ Definition of done

- [ ] `MethodsDemo` console project created with `dotnet new console`
- [ ] `CalculateCircleArea(double radius)` returns `Math.PI * radius * radius`
- [ ] `CalculateTrapezoidArea(double a, double b, double height)` returns `(a + b) / 2 * height`
- [ ] `Main` reads input, validates it with `double.TryParse`, and calls both methods
- [ ] Results are printed with two-decimal formatting (`:F2`)
- [ ] `dotnet run` produces the expected circle and trapezoid areas

---

## 🔑 Key concepts

- **Method definition & signature** — `static double CalculateCircleArea(double radius)` names the
  method, declares its return type, and lists the parameters it accepts.
- **Parameters in, value out** — a method receives data through its parameters and hands a single
  result back via `return`, keeping the calculation isolated from input/output.
- **Reuse & single responsibility** — each method does exactly one job, so it can be called from
  anywhere and tested on its own, which is the whole reason to factor logic into methods.
- **Safe parsing with `double.TryParse`** — convert user input without throwing; `TryParse` reports
  success as a `bool` and writes the parsed value to an `out` variable.
- **Output formatting** — `{area:F2}` formats a `double` to two decimal places for a clean,
  readable result.
