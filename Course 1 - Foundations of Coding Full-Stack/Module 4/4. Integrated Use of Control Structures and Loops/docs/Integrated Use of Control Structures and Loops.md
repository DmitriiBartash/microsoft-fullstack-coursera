# Integrated Use of Control Structures and Loops

**Course 1 — Foundations of Coding Full-Stack** · Module 4 · Lesson 4 · `You Try It!`

> Build a small C# console app that **combines decision-making with iteration**. Across four
> self-contained steps you'll pair `if-else` and `switch` with `do-while`, `for`, and `foreach`
> loops — validating input, grading scores, and routing order statuses — then wire every step
> together from `Program.cs`.

---

## 🎯 Objective

By the end of this activity, you will be able to combine `if-else` and `switch` control
structures with loops to solve practical coding problems — writing simple programs that
handle **decision-making inside iteration**.

---

## 🗂️ What you will build

A single console project named **`UseOfControlStructuresAndLoops`** where each exercise lives in
its own class and is invoked from `Main`:

| File                      | What it demonstrates                                                       |
| ------------------------- | -------------------------------------------------------------------------- |
| `Step1IfElseWithLoops.cs` | `do-while` loop + `if-else` to validate an even number between 1 and 10    |
| `Step2EvaluateGrades.cs`  | `for` loop + `if-else` to mark each grade **Pass** / **Fail**              |
| `Step3SwitchWithLoops.cs` | `foreach` loop + `switch` to print a message per order status              |
| `Step4GradesWithSwitch.cs`| `for` loop + `switch` to map a numeric score to a letter grade             |
| `Program.cs`              | Orchestrates: calls each step's `Run()` method in order                    |

**Flow:** `Program.Main → Step1.Run() → Step2.Run() → Step3.Run() → Step4.Run()`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version` (this code uses C# 12 collection
  expressions, so target **.NET 8** or newer)
- A code editor such as Visual Studio Code
- Comfort with basic C# syntax (variables, arrays, `Console.WriteLine`)

---

## 🛠️ Steps

### Step 0 — Scaffold the project

Create the console app and open it:

```bash
dotnet new console -n UseOfControlStructuresAndLoops
cd UseOfControlStructuresAndLoops
```

Each step below is a separate `.cs` file in this project. The namespace is shared so `Program.cs`
can call every step.

### Step 1 — Integrate `if-else` with a loop

Create a program that repeatedly asks the user for a number between 1 and 10 and ensures the
number is **even**. The loop continues until the user enters valid input, using an `if-else`
statement to check validity.

**Instructions**

- Use a `do-while` loop to continuously prompt the user for an even number between 1 and 10.
- Inside the loop, use an `if-else` statement to validate whether the number is even and between
  1 and 10.
- If the input is valid, print the number and exit the loop using `break`. If it's invalid,
  display an error message and repeat the prompt.

Create **`Step1IfElseWithLoops.cs`**:

```csharp
namespace UseOfControlStructuresAndLoops
{
    public class Step1IfElseWithLoops
    {
        public static void Run()
        {
            do
            {
                Console.Write("Enter an even number between 1 and 10: ");
                string input = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(input, out int number))
                {
                    if (number >= 1 && number <= 10 && number % 2 == 0)
                    {
                        Console.WriteLine("Valid input: " + number);
                        break; // Exit loop if valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. The number must be even and between 1 and 10.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a numeric value.");
                }
            } while (true);
        }
    }
}
```

### Step 2 — Use `if-else` to evaluate grades

Write a program that uses a `for` loop and an `if-else` structure to evaluate a list of student
grades. For each grade, determine whether the student has **passed** or **failed** based on the
grade value.

**Instructions**

- Define an array named `grades` containing a list of student grades.
- Use a `for` loop to iterate over each grade in the array.
- Inside the loop, use an `if-else` statement to check if each grade is greater than or equal to
  `65` (passing). Print `"Pass"` if the grade is passing and `"Fail"` if it is not.

Create **`Step2EvaluateGrades.cs`**:

```csharp
namespace UseOfControlStructuresAndLoops
{
    public class Step2EvaluateGrades
    {
        public static void Run()
        {
            int[] grades = [85, 42, 73, 64, 90, 58, 67];
            for (int i = 0; i < grades.Length; i++)
            {
                int grade = grades[i];
                if (grade >= 65)
                {
                    Console.WriteLine("Grade: " + grade + " - Pass");
                }
                else
                {
                    Console.WriteLine("Grade: " + grade + " - Fail");
                }
            }
        }
    }
}
```

### Step 3 — Integrate `switch` statements with loops

Create a program that processes multiple orders by their status. Each order can be `"Pending"`,
`"Shipped"`, `"Delivered"`, or `"Cancelled"`, and the program prints a message based on the status
of each order.

**Instructions**

- Define an array named `orderStatuses` containing the different statuses.
- Use a loop to iterate through the list of order statuses.
- Inside the loop, use a `switch` statement to print a different message based on the order's
  status. Include a `default` case to handle any unexpected value.

Create **`Step3SwitchWithLoops.cs`**:

```csharp
namespace UseOfControlStructuresAndLoops
{
    public class Step3SwitchWithLoops
    {
        public static void Run()
        {
            string[] orderStatuses = ["Pending", "Shipped", "Delivered", "Cancelled", "Unknown"];
            foreach (string status in orderStatuses)
            {
                switch (status)
                {
                    case "Pending":
                        Console.WriteLine("Order is pending and will be processed soon.");
                        break;
                    case "Shipped":
                        Console.WriteLine("Order has been shipped.");
                        break;
                    case "Delivered":
                        Console.WriteLine("Order was delivered successfully.");
                        break;
                    case "Cancelled":
                        Console.WriteLine("Order has been cancelled.");
                        break;
                    default:
                        Console.WriteLine("Unknown order status: " + status);
                        break;
                }
            }
        }
    }
}
```

### Step 4 — Evaluate student grades with `switch` and a loop

Write a program that uses a `for` loop to iterate over a list of student scores and a `switch`
statement to assign letter grades based on the score.

**Instructions**

- Define an array of student scores.
- Use a `for` loop to iterate through each score.
- Inside the loop, use a `switch` statement to assign a letter grade (A, B, C, D, F) based on the
  score. Dividing the score by 10 collapses each 10-point band into a single `case`.

Create **`Step4GradesWithSwitch.cs`**:

```csharp
namespace UseOfControlStructuresAndLoops
{
    public class Step4GradesWithSwitch
    {
        public static void Run()
        {
            int[] scores = [95, 82, 76, 64, 58, 89, 70];
            for (int i = 0; i < scores.Length; i++)
            {
                int score = scores[i];
                string letterGrade;
                // Divide score by 10 to simplify switch logic
                switch (score / 10)
                {
                    case 10:
                    case 9:
                        letterGrade = "A";
                        break;
                    case 8:
                        letterGrade = "B";
                        break;
                    case 7:
                        letterGrade = "C";
                        break;
                    case 6:
                        letterGrade = "D";
                        break;
                    default:
                        letterGrade = "F";
                        break;
                }
                Console.WriteLine($"Score: {score} - Grade: {letterGrade}");
            }
        }
    }
}
```

### Step 5 — Wire everything together in `Program.cs`

Replace the contents of **`Program.cs`** so `Main` calls each step in order, then run the app:

```csharp
using UseOfControlStructuresAndLoops;

class Program
{
    static void Main(string[] args)
    {
        // Call Step 1
        Step1IfElseWithLoops.Run();
        // Call Step 2
        Step2EvaluateGrades.Run();
        // Call Step 3
        Step3SwitchWithLoops.Run();
        // Call Step 4
        Step4GradesWithSwitch.Run();
    }
}
```

```bash
dotnet run
```

---

## ▶️ Expected result

Step 1 keeps prompting until you type a valid even number (e.g. `4`). The remaining steps run on
their fixed arrays and print:

```text
Valid input: 4
Grade: 85 - Pass
Grade: 42 - Fail
Grade: 73 - Pass
Grade: 64 - Fail
Grade: 90 - Pass
Grade: 58 - Fail
Grade: 67 - Pass
Order is pending and will be processed soon.
Order has been shipped.
Order was delivered successfully.
Order has been cancelled.
Unknown order status: Unknown
Score: 95 - Grade: A
Score: 82 - Grade: B
Score: 76 - Grade: C
Score: 64 - Grade: D
Score: 58 - Grade: F
Score: 89 - Grade: B
Score: 70 - Grade: C
```

---

## ☑️ Definition of done

- [ ] `UseOfControlStructuresAndLoops` console project created (.NET 8+)
- [ ] `Step1IfElseWithLoops` loops with `do-while` and validates even input via `if-else` + `break`
- [ ] `Step2EvaluateGrades` iterates `grades` with a `for` loop and prints Pass/Fail
- [ ] `Step3SwitchWithLoops` iterates `orderStatuses` with `foreach` and routes each via `switch`
- [ ] `Step4GradesWithSwitch` maps each score to a letter grade with `switch (score / 10)`
- [ ] `Program.cs` calls all four `Run()` methods and `dotnet run` produces the expected output

---

## 🔑 Key concepts

- **Decision-making inside iteration** — a loop supplies the values; the `if-else` or `switch`
  inside it decides what to do with each one. This pairing is the backbone of most input and data
  processing.
- **`do-while` for input validation** — a `do-while` runs the body at least once, so it's the
  natural fit for "keep asking until the input is valid," with `break` to leave on success.
- **`if-else` vs. `switch`** — use `if-else` for ranges and boolean conditions (`grade >= 65`);
  reach for `switch` when branching on a fixed set of discrete values (a status string, a score
  band).
- **Always handle the unexpected** — `int.TryParse` guards against non-numeric input, and a
  `switch` `default` (or `else`) branch catches values you didn't plan for, keeping the program
  robust.
- **Fall-through cases** — stacking `case 10:` and `case 9:` lets several labels share one body,
  a clean way to group equivalent inputs.
