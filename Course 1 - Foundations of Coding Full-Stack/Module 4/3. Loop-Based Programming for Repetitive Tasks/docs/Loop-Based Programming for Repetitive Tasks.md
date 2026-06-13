# Loop-Based Programming for Repetitive Tasks

**Course 1 — Foundations of Coding Full-Stack** · Module 4 · Lesson 3 · `You Try It!`

> Build a single C# console app (`Loop_Based`) that automates four repetitive tasks.
> Each task lives in its own class and uses a **loop** — `for` or `while` — combined with
> a control structure (`if-else`, `switch`). `Program.Main` runs all four in sequence so
> you can see loops driving real work end-to-end.

---

## 🎯 Objective

By the end of this activity, you will be able to write simple loop-based programs using
`for` and `while` loops, and combine them with control structures like `if-else` and
`switch` statements to automate repetitive tasks.

---

## 🗂️ What you will build

A console project named **`Loop_Based`** made of five files:

| File                        | Loop + control structure        | What it does                                        |
| --------------------------- | ------------------------------- | --------------------------------------------------- |
| `Step1TotalScore.cs`        | `for`                           | Sums an array of quiz scores                         |
| `Step2CalcFactorials.cs`    | `while`                         | Calculates the factorial of a number                |
| `Step3PassOrFail.cs`        | `for` + `if-else`               | Marks each student score Pass or Fail               |
| `Step4ComboLoopsSwitch.cs`  | `for` + `switch`                | Assigns a task to each weekday                       |
| `Program.cs`                | —                               | Runs all four tasks in order from `Main`            |

**Flow:** `Program.Main → Step1 → Step2 → Step3 → Step4 → console output`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (or any C# editor)
- Basic familiarity with C# arrays and `Console.WriteLine()`

---

## 🛠️ Steps

### Step 1 — Using a `for` loop to calculate total scores

You are developing a program for a quiz system to calculate the total score of a student
based on individual quiz scores. The scores are stored in an array, and you use a `for`
loop to sum them up.

**Instructions**

- Define an array named `scores` containing the integers `85`, `90`, `78`, `92`, and `88`.
- Use a `for` loop to iterate over each element in the array and calculate the total score.
- Print the total score using `Console.WriteLine()`.

```csharp
namespace Loop_Based
{
    public class Step1TotalScore
    {
        public static void Run()
        {
            int[] scores = [85, 90, 78, 92, 88];
            int totalScore = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                totalScore += scores[i];
            }
            Console.WriteLine($"Total Score: {totalScore}");
        }
    }
}
```

### Step 2 — Using a `while` loop to calculate factorials

Create a program that calculates the factorial of a given number using a `while` loop.
Declare the number, then multiply down to 1, decrementing after each iteration.

**Instructions**

- Declare an integer variable `number` and set its value to `5`.
- Use a `while` loop to calculate the factorial of the number.
- After each iteration, decrement the value of `number`.
- Print the factorial using `Console.WriteLine()`.

```csharp
namespace Loop_Based
{
    public class Step2CalcFactorials
    {
        public static void Run()
        {
            int number = 5;
            int factorial = 1;
            while (number > 0)
            {
                factorial *= number;
                number--;
            }
            Console.WriteLine($"Factorial: {factorial}");
        }
    }
}
```

### Step 3 — Combining loops and `if-else` to determine pass or fail

Write a program that uses a `for` loop with an `if-else` structure to check if each
student's score meets the passing criteria. A student passes if their score is `50` or above.

**Instructions**

- Define an array named `studentScores` containing the integers `45`, `60`, `72`, `38`, and `55`.
- Use a `for` loop to iterate over each element in the array.
- Inside the loop, use an `if-else` statement to check if the score is `50` or above.
- Print `Pass` if the score is `50` or above; otherwise, print `Fail`.

```csharp
namespace Loop_Based
{
    public class Step3PassOrFail
    {
        public static void Run()
        {
            int[] studentScores = { 45, 60, 72, 38, 55 };
            for (int i = 0; i < studentScores.Length; i++)
            {
                if (studentScores[i] >= 50)
                {
                    Console.WriteLine($"Score: {studentScores[i]} - Pass");
                }
                else
                {
                    Console.WriteLine($"Score: {studentScores[i]} - Fail");
                }
            }
        }
    }
}
```

### Step 4 — Combining loops and `switch` statements for task scheduling

Create a program that schedules weekly tasks using a `switch` statement inside a `for`
loop to assign a task for each day.

**Instructions**

- Define an array named `weekDays` containing the strings `"Monday"`, `"Tuesday"`, `"Wednesday"`, `"Thursday"`, `"Friday"`.
- Use a `for` loop to iterate over each element in the array.
- Inside the loop, use a `switch` statement to assign a task to each day:
  - If it's `"Monday"`, print `Team Meeting`.
  - If it's `"Tuesday"`, print `Code Review`.
  - If it's `"Wednesday"`, print `Development`.
  - If it's `"Thursday"`, print `Testing`.
  - If it's `"Friday"`, print `Deployment`.

```csharp
namespace Loop_Based
{
    public class Step4ComboLoopsSwitch
    {
        public static void Run()
        {
            string[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            for (int i = 0; i < weekDays.Length; i++)
            {
                string day = weekDays[i];
                Console.Write($"{day}: ");
                switch (day)
                {
                    case "Monday":
                        Console.WriteLine("Team Meeting");
                        break;
                    case "Tuesday":
                        Console.WriteLine("Code Review");
                        break;
                    case "Wednesday":
                        Console.WriteLine("Development");
                        break;
                    case "Thursday":
                        Console.WriteLine("Testing");
                        break;
                    case "Friday":
                        Console.WriteLine("Deployment");
                        break;
                    default:
                        Console.WriteLine("No task assigned.");
                        break;
                }
            }
        }
    }
}
```

### Step 5 — Wire it all together in `Main`

`Program.Main` calls each task's `Run()` method in order, printing a header before each one
so the output is easy to read.

```csharp
namespace Loop_Based
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Task 1: Total Score");
            Step1TotalScore.Run();

            Console.WriteLine("\nRunning Task 2: Factorial");
            Step2CalcFactorials.Run();

            Console.WriteLine("\nRunning Task 3: Pass or Fail");
            Step3PassOrFail.Run();

            Console.WriteLine("\nRunning Task 4: Weekly Task Scheduler");
            Step4ComboLoopsSwitch.Run();
        }
    }
}
```

Scaffold the project, drop in the five files above, and run it:

```bash
dotnet new console -n Loop_Based
cd Loop_Based
dotnet run
```

---

## ▶️ Expected result

Running the app prints the result of each task in sequence:

```text
Running Task 1: Total Score
Total Score: 433

Running Task 2: Factorial
Factorial: 120

Running Task 3: Pass or Fail
Score: 45 - Fail
Score: 60 - Pass
Score: 72 - Pass
Score: 38 - Fail
Score: 55 - Pass

Running Task 4: Weekly Task Scheduler
Monday: Team Meeting
Tuesday: Code Review
Wednesday: Development
Thursday: Testing
Friday: Deployment
```

---

## ☑️ Definition of done

- [ ] `Loop_Based` console project created and runs with `dotnet run`
- [ ] `Step1TotalScore` sums `scores` with a `for` loop and prints `Total Score: 433`
- [ ] `Step2CalcFactorials` computes `5!` with a `while` loop and prints `Factorial: 120`
- [ ] `Step3PassOrFail` uses `for` + `if-else` to print Pass/Fail for each score
- [ ] `Step4ComboLoopsSwitch` uses `for` + `switch` to print a task per weekday
- [ ] `Program.Main` calls all four `Run()` methods in order with labelled headers

---

## 🔑 Key concepts

- **`for` vs `while`** — use a `for` loop when you know the count (iterating an array by index); use a `while` loop when you loop until a condition changes (counting `number` down to 0).
- **Loops + branching** — a loop handles the *repetition*; an `if-else` or `switch` inside it handles the *decision* on each item, which is the core pattern for automating per-element work.
- **`switch` for discrete cases** — a `switch` maps a fixed set of values (the weekdays) to actions more clearly than a long `if-else` chain, and a `default` case handles anything unexpected.
- **Accumulator pattern** — both the score sum (`totalScore += ...`) and the factorial (`factorial *= ...`) carry a running value across iterations, initialized before the loop (`0` for a sum, `1` for a product).
- **One responsibility per class** — splitting each task into its own class with a `Run()` method keeps `Main` readable and lets you test or reuse each piece independently.
