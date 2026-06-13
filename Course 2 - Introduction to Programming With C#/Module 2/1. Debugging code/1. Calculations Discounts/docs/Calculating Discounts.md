# Calculating Discounts

**Course 2 — Introduction to Programming With C#** · Module 2 · Lesson 1 · `You Try It!`

> Debug a small C# method that should apply a percentage discount to a product price but
> instead returns the wrong number. The bug is **logical**, not a crash: the code compiles
> and runs, yet the math is wrong. You'll diagnose *why*, then fix it.

---

## 🎯 Objective

Practice basic debugging techniques — setting a breakpoint, inspecting variable values, and
stepping through code — to find and fix a **logical error** in an `ApplyDiscount` method, and
harden it with input validation so it fails fast on bad data.

---

## 🧾 What you'll produce

A corrected `Program.cs` in your console application whose `ApplyDiscount` method:

| Concern              | Behaviour                                                              |
| -------------------- | --------------------------------------------------------------------- |
| Discount math        | Treats `discountPercentage` as a **percentage**, not a flat amount    |
| Result for `(1000, 15)` | Prints `The final price is: 850`                                    |
| Bad input            | Throws `ArgumentException` for a negative price or a percentage outside 0–100 |
| Failure mode         | Wrapped in `try/catch` so the program reports the error instead of crashing |

---

## ✅ Prerequisites

- The Visual Studio Code **console application** you created at the start of the course
- .NET SDK installed — check with `dotnet --version`
- Clear any existing code out of `Program.cs` before pasting in the snippets below

---

## 🔍 Walkthrough

### Step 1 — Read the broken code

The method below is *meant* to calculate the final price after applying a discount, but it
produces the wrong output:

```csharp
public class Program
{
    // Method to calculate the final price after a discount
    public static double ApplyDiscount(double price, double discountPercentage)
    {
        return price - discountPercentage;
    }

    public static void Main()
    {
        double finalPrice = ApplyDiscount(1000, 15);
        Console.WriteLine("The final price is: " + finalPrice);
    }
}
```

### Step 2 — Reproduce the symptom

Run it. For `ApplyDiscount(1000, 15)` you expect a 15 % discount → `850`, but the program
prints:

```text
The final price is: 985
```

The code **compiles and runs** — there's no exception. That tells you this is a *logical*
error (wrong result), not a *syntax* or *runtime* error.

### Step 3 — Inspect with the debugger

Set a breakpoint on the `return` line, then run with debugging (F5) and **step through**:

- Hover over `price` → `1000`
- Hover over `discountPercentage` → `15`
- The expression evaluated is `1000 - 15`, which is `985`

Inspecting the values makes the mistake obvious: `15` is being subtracted as if it were
**15 currency units**, when it actually means **15 percent**.

### Step 4 — Fix the logic

A percentage discount has to be converted into an *amount* before it is subtracted:

```csharp
double discountAmount = price * (discountPercentage / 100);
return price - discountAmount;
```

Now `1000 * (15 / 100)` = `150`, and `1000 - 150` = **`850`** — the expected result.

### Step 5 — Harden and confirm

While you're here, guard against nonsensical input so the method *fails fast*. Here is the
complete corrected program:

```csharp
public class Program
{
    // Method to calculate the final price after applying a percentage discount
    public static double ApplyDiscount(double price, double discountPercentage)
    {
        // Error checks
        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }
        if (discountPercentage < 0 || discountPercentage > 100)
        {
            throw new ArgumentException("Discount percentage must be between 0 and 100.");
        }

        // Calculate the discount
        double discountAmount = price * (discountPercentage / 100);
        return price - discountAmount;
    }

    public static void Main()
    {
        try
        {
            double finalPrice = ApplyDiscount(1000, 15); // Valid input
            Console.WriteLine("The final price is: " + finalPrice);

            // Uncomment to test error handling:
            // double invalid = ApplyDiscount(-200, 10);   // Negative price
            // double invalid = ApplyDiscount(500, 150);   // Invalid discount percentage
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
```

Run it again:

```bash
dotnet run
```

You should now see `The final price is: 850`. Uncomment either invalid call to watch the
`try/catch` report the problem cleanly instead of letting the program misbehave.

---

## 🧪 Your turn

Make sure you can explain the bug, not just patch it:

- **Classify it.** Was this a syntax, runtime, or logical error? Why does that category fit?
- **Predict before you run.** What does `ApplyDiscount(500, 150)` do with the error checks in
  place — and what *would* it have returned without them?
- **Step it through.** Set a breakpoint and confirm `discountAmount` is `150` for the
  `(1000, 15)` call before the method returns.

---

## 🌟 What good looks like

- The program prints **`The final price is: 850`** for `ApplyDiscount(1000, 15)`.
- You can point to the exact line that was wrong (`return price - discountPercentage;`) and
  state the fix (multiply by `discountPercentage / 100` first).
- Invalid input raises a clear `ArgumentException` that the `try/catch` turns into a friendly
  `Error: …` message rather than a crash.

---

## ☑️ Definition of done

- [ ] Old code removed from `Program.cs` and the corrected version pasted in
- [ ] The discount is computed as `price * (discountPercentage / 100)`, not a flat subtraction
- [ ] `ApplyDiscount(1000, 15)` outputs `850`
- [ ] Negative price and out-of-range percentage both throw `ArgumentException`
- [ ] `dotnet run` completes without an unhandled exception on valid input

---

## 🔑 Key concepts

- **Logical errors are silent** — the code runs to completion and still produces the wrong
  answer, so the compiler can't catch them; you find them by checking results against
  expectations.
- **Breakpoints + variable inspection** — pausing on the suspect line and reading the live
  values (`price`, `discountPercentage`, `discountAmount`) turns a mystery into an obvious
  off-by-meaning mistake.
- **Units must match the math** — a percentage has to be converted to an amount
  (`× percentage / 100`) before it can be subtracted from a price.
- **Fail fast with validation** — rejecting a negative price or an out-of-range percentage up
  front, via `ArgumentException`, stops bad data from silently producing nonsense.
- **`try/catch` for graceful failure** — wrapping the call lets the program *report* an error
  instead of terminating abruptly, which is far easier to debug and to use.
