# Introduction to Functions in Programming

**Course 1 — Foundations of Coding Full-Stack** · Module 5 · Lesson 1 · `You Try It!`

> Use **functions** to package logic into reusable, named blocks. In one small C#
> console app you will write two functions — one that computes the **area of a circle**
> and one that computes the **area of a trapezoid** — prompt the user for input, call
> each function, and print the formatted result.

---

## 🎯 Objective

Use functions to create reusable code blocks: define `CalculateCircleArea` and
`CalculateTrapezoidArea`, then call them from `Main` to compute and display each area
from user-supplied values.

---

## 🗂️ What you will build

A single C# console program with two helper functions wired up in `Main`:

| Function                                          | Input parameters            | Returns                              |
| ------------------------------------------------- | --------------------------- | ------------------------------------ |
| `CalculateCircleArea(double radius)`              | `radius`                    | Circle area — `Math.PI * r * r`      |
| `CalculateTrapezoidArea(double a, double b, double height)` | `a`, `b`, `height` | Trapezoid area — `(a + b) / 2 * height` |
| `Main()`                                          | —                           | Reads input, calls both, prints results |

**Flow:** `prompt user  →  read values  →  Calculate…Area(values)  →  area  →  Console.WriteLine`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor such as Visual Studio Code
- Basic familiarity with `Console.Write` / `Console.ReadLine` for console I/O

---

## 🛠️ Steps

### Step 1 — Scaffold the console app

Create a new console project and move into it.

```bash
dotnet new console -n FunctionsDemo
cd FunctionsDemo
```

Open `Program.cs` and clear any existing content.

### Step 2 — Problem 1: a function for circle area

**Problem statement:** write a function to calculate the area of a circle. It accepts one
input parameter — the **radius**. The program prompts the user for this value, uses the
function to compute the area, and displays the result.

**Formula:** the area of a circle is `π * r²`, where `r` is the radius. For π, use `Math.PI`.

```csharp
// Function to calculate the area of a circle
static double CalculateCircleArea(double radius)
{
    return Math.PI * radius * radius;
}
```

### Step 3 — Problem 2: a function for trapezoid area

**Problem statement:** write a function to calculate the area of a trapezoid. It accepts
three input parameters — the lengths of the two parallel sides (`a` and `b`) and the
**height**. The program prompts the user for these values, uses the function to compute
the area, and displays the result.

**Formula:** the area of a trapezoid is `(a + b) / 2 * height`.

```csharp
// Function to calculate the area of a trapezoid
static double CalculateTrapezoidArea(double a, double b, double height)
{
    return (a + b) / 2 * height;
}
```

### Step 4 — Wire everything up in `Main`

Prompt the user, read each value with `Console.ReadLine`, convert it to a `double`, call
the matching function, and print the result formatted to two decimal places (`:F2`).

```csharp
public class Program
{
    // Function to calculate the area of a circle
    static double CalculateCircleArea(double radius)
    {
        return Math.PI * radius * radius;
    }

    // Function to calculate the area of a trapezoid
    static double CalculateTrapezoidArea(double a, double b, double height)
    {
        return (a + b) / 2 * height;
    }

    public static void Main()
    {
        // PROBLEM 1: Circle Area
        Console.Write("Enter the radius of the circle: ");
        double radius = Convert.ToDouble(Console.ReadLine());
        double circleArea = CalculateCircleArea(radius);
        Console.WriteLine($"The area of the circle with radius {radius} is {circleArea:F2}");

        // PROBLEM 2: Trapezoid Area
        Console.Write("\nEnter the length of side a of the trapezoid: ");
        double a = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the length of side b of the trapezoid: ");
        double b = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the height of the trapezoid: ");
        double height = Convert.ToDouble(Console.ReadLine());
        double trapezoidArea = CalculateTrapezoidArea(a, b, height);
        Console.WriteLine($"The area of the trapezoid is {trapezoidArea:F2}");
    }
}
```

### Step 5 — Run it

```bash
dotnet run
```

Enter the values when prompted and confirm the printed areas.

---

## ▶️ Expected result

With radius `5`, then `a = 4`, `b = 6`, `height = 3`, the program prints:

```text
Enter the radius of the circle: 5
The area of the circle with radius 5 is 78.54

Enter the length of side a of the trapezoid: 4
Enter the length of side b of the trapezoid: 6
Enter the height of the trapezoid: 3
The area of the trapezoid is 15.00
```

---

## ☑️ Definition of done

- [ ] `FunctionsDemo` console project created and `Program.cs` cleared
- [ ] `CalculateCircleArea(double radius)` returns `Math.PI * radius * radius`
- [ ] `CalculateTrapezoidArea(double a, double b, double height)` returns `(a + b) / 2 * height`
- [ ] `Main` prompts for input, calls both functions, and prints each area with `:F2`
- [ ] `dotnet run` produces the circle and trapezoid areas from the entered values

---

## 🔑 Key concepts

- **Functions = reusable blocks** — naming a calculation (`CalculateCircleArea`) lets you
  call it anywhere instead of repeating the formula, the heart of this exercise.
- **Parameters and return values** — each function declares its inputs (`radius`; `a`, `b`,
  `height`) and hands back a single `double` result the caller can use.
- **Separation of concerns** — the formulas live in their own functions while `Main` handles
  user I/O, so each piece stays small and focused.
- **Reading console input** — `Console.ReadLine` returns a string, so `Convert.ToDouble`
  turns it into a number before the math.
- **Output formatting** — the `:F2` format specifier renders the result to two decimal
  places for a clean, readable answer.
