# Finding the Maximum Number in an Array

**Course 2 — Introduction to Programming With C#** · Module 2 · Lesson 1 · `You Try It!`

> Debug a C# `FindMax` method that returns the **wrong answer** when every value
> in the array is negative. You'll spot the **logical error** (`int max = 0`),
> reason about *why* it bites, fix it, then harden the method with guard clauses
> and `try`/`catch` so it fails fast on bad input.

---

## 🎯 Objective

Practice core debugging techniques — reading code critically, predicting output,
inspecting variable values, and stepping through a loop — to **find and fix a logical
error**, then make the corrected method robust against null, empty, and out-of-range input.

---

## 🗂️ What you will produce

A single console program (`Program.cs`) that evolves through the exercise:

| Stage              | State of `FindMax`                                               |
| ------------------ | --------------------------------------------------------------- |
| Starting code      | Has a **logical bug** — seeds `max` with `0`                    |
| Fixed code         | Seeds `max` with `numbers[0]`, loops from index `1`            |
| Hardened code      | Adds guard clauses + `try`/`catch` for null, empty, overflow   |

**Flow:** `array  →  FindMax(numbers)  →  max  →  Console.WriteLine`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- The console application you created at the start of the course (open it, then **clear** `Program.cs`)

---

## 🛠️ Walkthrough

### Step 1 — Set up your environment

Use the Visual Studio Code console application from the start of the course. Remove any
existing code in `Program.cs`, then copy in the starting code below to follow along.

### Step 2 — Read the buggy starting code

This code *tries* to find the maximum number in an array, but it has a **logical error**
that produces an incorrect result when **all the numbers are negative**.

```csharp
public class Program
{
    public static int FindMax(int[] numbers)
    {
        int max = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
            {
                max = numbers[i];
            }
        }
        return max;
    }

    public static void Main()
    {
        int[] myNumbers = { -5, -10, -3, -8, -2 };
        int maxNumber = FindMax(myNumbers);
        Console.WriteLine("The maximum number is: " + maxNumber);
    }
}
```

### Step 3 — Find the logical error

Step through the loop in your head (or with the debugger) for `{ -5, -10, -3, -8, -2 }`:

- `max` starts at `0`.
- Every element is negative, so `numbers[i] > max` is **never** true.
- The `if` body never runs, `max` is never updated, and the method returns `0`.

```text
The maximum number is: 0     ← wrong: 0 is not even in the array
```

The seed value `0` is the bug. It silently injects a number that isn't part of the
data. The expected answer is `-2` (the largest of the negatives).

> 💡 **Set a breakpoint** on the `if (numbers[i] > max)` line and watch `max` and
> `numbers[i]` as you step. You'll see `max` stay at `0` the whole way through —
> that's your evidence.

### Step 4 — Fix the seed and the loop bounds

Initialize `max` to the **first element** of the array instead of `0`, then start the
loop at index `1`. Now `max` always holds a real value from the data.

```csharp
public static int FindMax(int[] numbers)
{
    int max = numbers[0];                       // seed with real data, not 0
    for (int i = 1; i < numbers.Length; i++)    // start from the second element
    {
        if (numbers[i] > max)
        {
            max = numbers[i];
        }
    }
    return max;
}
```

Re-run with the same array and you now get the correct result:

```text
The maximum number is: -2
```

### Step 5 — Harden the method against bad input

Seeding with `numbers[0]` assumes the array is non-empty and non-null, so add **guard
clauses** that fail fast, plus a `try`/`catch` in `Main` to report errors cleanly.

```csharp
public class Program
{
    // Method to find the maximum number in an array
    public static int FindMax(int[] numbers)
    {
        // Error check 1: array is null
        if (numbers == null)
        {
            throw new ArgumentNullException(nameof(numbers), "Input array cannot be null.");
        }
        // Error check 2: array is empty
        if (numbers.Length == 0)
        {
            throw new ArgumentException("The array cannot be empty.");
        }
        // Initialize max to the first element
        int max = numbers[0];
        // Loop through the array starting from the second element
        for (int i = 1; i < numbers.Length; i++)
        {
            // Error check 3: check for int.MinValue overflow
            if (numbers[i] < int.MinValue || numbers[i] > int.MaxValue)
            {
                throw new OverflowException("Array contains an out-of-range integer.");
            }
            if (numbers[i] > max)
            {
                max = numbers[i];
            }
        }
        return max;
    }

    public static void Main()
    {
        try
        {
            // Test array
            int[] myNumbers = { -5, -10, -3, -8, -2 };
            // Uncomment the following lines one at a time to test different errors:
            // int[] myNumbers = null;             // Triggers ArgumentNullException
            // int[] myNumbers = { };              // Triggers ArgumentException
            // int[] myNumbers = new int[1000000]; // Simulated size check (if needed)
            int maxNumber = FindMax(myNumbers);
            Console.WriteLine("The maximum number is: " + maxNumber);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine("Null Error: " + ex.Message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Argument Error: " + ex.Message);
        }
        catch (OverflowException ex)
        {
            Console.WriteLine("Overflow Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected Error: " + ex.Message);
        }
    }
}
```

> 📝 The `int.MinValue`/`int.MaxValue` check can never be `true` for an `int` (every
> `int` is in range by definition) — it's a deliberate **placeholder** showing *where*
> a range check would go if you later widened the input type or parsed values yourself.

---

## ▶️ Expected result

Run the program and confirm the fix:

```bash
dotnet run
```

```text
The maximum number is: -2
```

Uncomment the `null` or `{ }` test line and you'll instead see a clean, caught message
(`Null Error: …` or `Argument Error: …`) rather than an unhandled crash.

---

## ☑️ Definition of done

- [ ] You can explain **why** the original returns `0` for an all-negative array
- [ ] `max` is seeded with `numbers[0]` and the loop starts at index `1`
- [ ] Running on `{ -5, -10, -3, -8, -2 }` prints `The maximum number is: -2`
- [ ] Guard clauses throw `ArgumentNullException` / `ArgumentException` for null / empty input
- [ ] `Main` wraps the call in `try`/`catch` and reports each error type distinctly

---

## 🔑 Key concepts

- **Logical errors run silently** — the buggy code compiles and *executes* fine; it just
  returns the wrong value. Only checking output against an expected answer reveals it.
- **Never seed an aggregate with a magic constant** — initializing `max` to `0` assumes
  the answer is at least `0`. Seed from the data itself (`numbers[0]`) so every input value
  can win.
- **Off-by-one awareness** — once `max = numbers[0]`, the loop must start at `i = 1`;
  re-comparing element `0` against itself is harmless here but signals sloppy bounds.
- **Fail fast with guard clauses** — validate null and empty *before* touching `numbers[0]`,
  so the method throws a clear exception instead of an `IndexOutOfRangeException`.
- **Debugger as evidence** — breakpoints and variable inspection turn "I think it's wrong"
  into "I can see `max` never changes," which is how you locate a logical bug.
