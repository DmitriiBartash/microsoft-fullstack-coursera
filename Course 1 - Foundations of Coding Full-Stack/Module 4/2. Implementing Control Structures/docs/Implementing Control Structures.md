# Implementing Control Structures

**Course 1 — Foundations of Coding Full-Stack** · Module 4 · Lesson 2 · `You Try It!`

> Practice driving program flow with **decision logic**. You'll build two small C#
> console programs: a gym **membership-fee calculator** using nested `if`/`else if`/`else`,
> and a bank **account manager** using a `switch` statement — picking the right control
> structure for each shape of decision.

---

## 🎯 Objective

Solve decision-making scenarios using advanced `if-else` statements and complex `switch`
cases to control program flow by evaluating variable values and program state — applying
those skills to a gym membership fee system and a bank account management system.

---

## 🧩 What you'll produce

Two standalone C# console programs, each demonstrating a different control structure:

| Program                       | Namespace                | Control structure        | Decides by                       |
| ----------------------------- | ------------------------ | ------------------------ | -------------------------------- |
| Membership Fee Calculator     | `MembershipFeeCalcSys`   | Nested `if` / `else if`  | Age range **and** membership type|
| Bank Account Manager          | `BankAccountManagSyst`   | `switch` statement       | Account type (string)            |

**Rule of thumb:** use `if` / `else if` when branches test **ranges or compound conditions**;
use `switch` when you're matching a **single value against discrete cases**.

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor (Visual Studio Code or Visual Studio)
- Comfort reading console input with `Console.ReadLine()` and parsing with `int.TryParse`

---

## 🛠️ Walkthrough

### Problem 1 — Membership Fee Calculation System

**Scenario.** A gym charges a fee that depends on the user's **age** and **membership type**:

| Age group        | Basic | Premium |
| ---------------- | ----- | ------- |
| Under 18         | `$15` | `$25`   |
| 18 – 60          | `$30` | `$50`   |
| Over 60          | `$20` | `$35`   |

**Approach.** Each age band is a compound range condition (`age < 18`, `age >= 18 && age <= 60`,
`age > 60`), so an outer `if` / `else if` ladder selects the band, and an inner `if` / `else if`
selects the fee by membership type. Validate every input and fail fast on bad data.

```csharp
namespace MembershipFeeCalcSys
{
    public class Program
    {
        public static void Main()
        {
            // Step 1: Input age
            Console.WriteLine("Enter your age:");
            string? ageInput = Console.ReadLine();
            if (!int.TryParse(ageInput, out int age) || age < 0)
            {
                Console.WriteLine("Invalid age input");
                return;
            }

            // Step 2: Input membership type
            Console.WriteLine("Enter membership type (basic/premium):");
            string? membershipInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(membershipInput))
            {
                Console.WriteLine("Invalid membership type.");
                return;
            }
            string membershipType = membershipInput.Trim().ToLower();

            // Step 3: Decide the fee by age band, then by membership type
            double fee = 0;
            if (age < 18)
            {
                if (membershipType == "basic")
                {
                    fee = 15;
                }
                else if (membershipType == "premium")
                {
                    fee = 25;
                }
                else
                {
                    Console.WriteLine("Unknown membership type.");
                    return;
                }
            }
            else if (age >= 18 && age <= 60)
            {
                if (membershipType == "basic")
                {
                    fee = 30;
                }
                else if (membershipType == "premium")
                {
                    fee = 50;
                }
                else
                {
                    Console.WriteLine("Unknown membership type.");
                    return;
                }
            }
            else if (age > 60)
            {
                if (membershipType == "basic")
                {
                    fee = 20;
                }
                else if (membershipType == "premium")
                {
                    fee = 35;
                }
                else
                {
                    Console.WriteLine("Unknown membership type.");
                    return;
                }
            }

            // Step 4: Output the result
            Console.WriteLine($"The membership fee is: ${fee}");
        }
    }
}
```

> A 25-year-old choosing `premium` lands in the `18 – 60` band → the program prints
> `The membership fee is: $50`.

### Problem 2 — Bank Account Management System

**Scenario.** A banking app applies the correct fee or interest rate based on the account type:

| Account type | Effect                                            |
| ------------ | ------------------------------------------------- |
| `savings`    | Apply a **2%** interest rate                      |
| `checking`   | Apply a **$10** monthly fee                       |
| `business`   | Apply a **1%** interest rate **and** a $20 fee    |
| anything else| Display an error message                          |

**Approach.** Here we match a **single string** against a fixed set of values — the textbook
case for a `switch`. Each `case` updates the running `balance`, and the `default` arm catches
unknown account types.

```csharp
namespace BankAccountManagSyst
{
    public class Program
    {
        public static void Main()
        {
            // Step 1: Get user input for account type
            Console.WriteLine("Enter account type (savings/checking/business):");
            string? accountInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(accountInput))
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            string accountType = accountInput.Trim().ToLower();

            double balance = 10000.00; // Example for demonstration
            double interestRate = 0, monthlyFee = 0;

            // Step 2: Use switch-case to determine behavior
            switch (accountType)
            {
                case "savings":
                    interestRate = 0.02; // 2% interest
                    double interest = balance * interestRate;
                    balance += interest;
                    Console.WriteLine($"Savings account: +2% interest applied. New balance: ${balance:F2}");
                    break;
                case "checking":
                    monthlyFee = 10.0;
                    balance -= monthlyFee;
                    Console.WriteLine($"Checking account: $10 monthly fee deducted. New balance: ${balance:F2}");
                    break;
                case "business":
                    interestRate = 0.01; // 1% interest
                    monthlyFee = 20.0;
                    balance += balance * interestRate;
                    balance -= monthlyFee;
                    Console.WriteLine($"Business account: +1% interest and $20 fee applied. New balance: ${balance:F2}");
                    break;
                default:
                    Console.WriteLine("Unknown account type. Please enter savings, checking, or business.");
                    break;
            }
        }
    }
}
```

> Entering `savings` on a `$10,000.00` balance prints
> `Savings account: +2% interest applied. New balance: $10200.00`.

> **Note:** switch on the **normalized** `accountType` (trimmed + lower-cased), not the raw
> `accountInput`. Switching on the raw input would make `"Savings "` or `"SAVINGS"` fall through
> to `default` even though the user meant savings.

---

## 🙌 Your turn

- Run each program with `dotnet run` and try the **boundary** inputs: age `17`, `18`, `60`, `61`.
- Feed deliberately bad data (empty age, `"gold"` membership, `"crypto"` account) and confirm
  every invalid path prints a clear message and exits cleanly.
- Refactor Problem 1's fee table into a single `switch` expression or a lookup, and compare
  readability against the nested `if` ladder.

---

## ▶️ What good looks like

- Valid inputs print the correct fee / new balance for **every** branch in the tables above.
- Boundary ages (`17`/`18` and `60`/`61`) fall into the band you expect — no off-by-one gaps.
- Invalid or unknown inputs are rejected with a clear message instead of crashing or computing
  a wrong number.

---

## ☑️ Definition of done

- [ ] `MembershipFeeCalcSys` returns the right fee for all three age bands × both membership types
- [ ] Membership program validates age with `int.TryParse` and rejects negative / non-numeric input
- [ ] `BankAccountManagSyst` applies the correct interest and/or fee for `savings`, `checking`, `business`
- [ ] The `switch` matches on the **normalized** account type and handles unknown types via `default`
- [ ] Both programs run via `dotnet run` and handle empty / invalid input without throwing

---

## 🔑 Key concepts

- **Match the structure to the decision** — `if` / `else if` shines for **ranges and compound
  conditions** (`age >= 18 && age <= 60`); `switch` shines for matching **one value** against
  discrete cases.
- **Nesting layers decisions** — the membership program decides the age band first, then the
  fee within that band, mirroring the two-dimensional pricing table.
- **Validate and fail fast** — guard clauses with `int.TryParse` and `string.IsNullOrWhiteSpace`
  reject bad input early with an early `return`, before any calculation runs.
- **Normalize before you compare** — `Trim().ToLower()` makes string matching robust to casing
  and stray whitespace; always branch on the cleaned value.
- **Always cover the fallthrough** — a final `else` and a `switch` `default` make sure unexpected
  values are handled instead of silently doing nothing.
