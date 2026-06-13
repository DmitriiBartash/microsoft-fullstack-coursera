# Control Structures

**Course 1 — Foundations of Coding Full-Stack** · Module 4 · Lesson 1 · `You Try It!`

> Build a small C# console app for a travel company that makes decisions with
> **control structures**: an **`if-else`** chain prices a ticket by the passenger's
> age, and a **`switch`** statement books the chosen travel mode. Each behaviour
> lives in its own method, and `Main` runs them in turn.

---

## 🎯 Objective

By the end of this activity, you will be able to apply **`if-else` statements** and
**`switch` cases** to simulate decisions in a program — here, booking tickets for
different travel modes and pricing them by passenger age.

---

## 🗂️ What you will build

A single console project named **`Control_Structures`** whose `Program` class
exposes two decision routines, called from `Main`:

| Method                  | Control structure        | Decides                                              |
| ----------------------- | ------------------------ | --------------------------------------------------- |
| `TicketPricing()`       | `if` / `else if` / `else`| Ticket price band from the passenger's age          |
| `TravelModeSelection()` | `switch`                 | Booking message for the chosen mode of transport    |

**Flow:** `Main → TicketPricing() → if-else on age → TravelModeSelection() → switch on mode`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor (Visual Studio or Visual Studio Code)
- Basic familiarity with `Console.WriteLine` / `Console.ReadLine` for console I/O

---

## 🛠️ Steps

### Step 1 — Scaffold the console project

Create the project and move into it.

```bash
dotnet new console -n Control_Structures
cd Control_Structures
```

Open `Program.cs` and clear any existing content — you will replace it with the code below.

### Step 2 — If-else statement for ticket pricing

You are developing a program for a travel company to determine the ticket price based
on the passenger's age. The company offers discounted prices for children and seniors:

| Passenger group        | Age range      | Price                 |
| ---------------------- | -------------- | --------------------- |
| Children               | under 12       | Half price            |
| Adults                 | 12 to 65       | Full price            |
| Seniors                | over 65        | 20% discount          |

Build the routine so it:

- Prompts the user and reads the input with `Console.ReadLine()`.
- Converts the input to an `int age` (using `int.TryParse` so bad input is handled, not crashed).
- Uses an `if` / `else if` / `else` chain to print the right band:
  - `age < 12` → `"Half price ticket"`
  - `age` between 12 and 65 → `"Full price ticket"`
  - otherwise → `"Senior discount ticket"`

```csharp
static void TicketPricing()
{
    Console.Write("Please enter your age: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int age))
    {
        Console.WriteLine("Invalid input. Please enter a valid number.");
        return;
    }

    if (age < 0)
    {
        Console.WriteLine("Age cannot be negative.");
        return;
    }

    if (age < 12)
    {
        Console.WriteLine("Half price ticket");
    }
    else if (age < 65)
    {
        Console.WriteLine("Full price ticket");
    }
    else
    {
        Console.WriteLine("Senior discount ticket");
    }
}
```

### Step 3 — Switch statement for travel mode selection

The travel company offers multiple travel modes — `"Bus"`, `"Train"`, and `"Flight"` —
and each has a different booking message. Use a `switch` statement to decide which mode
the user selected.

Build the routine so it:

- Reads a `string mode` from the user with `Console.ReadLine()`.
- Normalises the input (`Trim().ToLower()`) so casing and stray spaces do not matter.
- Switches on the mode and prints the matching message:
  - `"Bus"` → `"Booking a bus ticket"`
  - `"Train"` → `"Booking a train ticket"`
  - `"Flight"` → `"Booking a flight ticket"`
- Includes a `default` case for invalid input: `"Invalid selection. Please choose Bus, Train, or Flight."`

```csharp
static void TravelModeSelection()
{
    Console.Write("Choose travel mode (Bus, Train, Flight): ");
    string? mode = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(mode))
    {
        Console.WriteLine("Input cannot be empty. Please enter a valid travel mode.");
        return;
    }

    switch (mode.Trim().ToLower())
    {
        case "bus":
            Console.WriteLine("Booking a bus ticket");
            break;
        case "train":
            Console.WriteLine("Booking a train ticket");
            break;
        case "flight":
            Console.WriteLine("Booking a flight ticket");
            break;
        default:
            Console.WriteLine("Invalid selection. Please choose Bus, Train, or Flight.");
            break;
    }
}
```

### Step 4 — Wire it together in `Main`

Call both routines from `Main` so the program runs the pricing decision first, then the
travel-mode booking. The complete `Program.cs`:

```csharp
namespace Control_Structures
{
    class Program
    {
        static void Main()
        {
            // Step 1: If-else statement for ticket pricing
            Console.WriteLine("=== Ticket Pricing Program ===");
            TicketPricing();

            // Step 2: Switch statement for travel mode selection
            Console.WriteLine("\n=== Second Program ===");
            TravelModeSelection();
        }

        static void TicketPricing()
        {
            Console.Write("Please enter your age: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int age))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            if (age < 0)
            {
                Console.WriteLine("Age cannot be negative.");
                return;
            }

            if (age < 12)
            {
                Console.WriteLine("Half price ticket");
            }
            else if (age < 65)
            {
                Console.WriteLine("Full price ticket");
            }
            else
            {
                Console.WriteLine("Senior discount ticket");
            }
        }

        static void TravelModeSelection()
        {
            Console.Write("Choose travel mode (Bus, Train, Flight): ");
            string? mode = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(mode))
            {
                Console.WriteLine("Input cannot be empty. Please enter a valid travel mode.");
                return;
            }

            switch (mode.Trim().ToLower())
            {
                case "bus":
                    Console.WriteLine("Booking a bus ticket");
                    break;
                case "train":
                    Console.WriteLine("Booking a train ticket");
                    break;
                case "flight":
                    Console.WriteLine("Booking a flight ticket");
                    break;
                default:
                    Console.WriteLine("Invalid selection. Please choose Bus, Train, or Flight.");
                    break;
            }
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

The program runs both decisions in sequence. A sample session:

```text
=== Ticket Pricing Program ===
Please enter your age: 70
Senior discount ticket

=== Second Program ===
Choose travel mode (Bus, Train, Flight): Train
Booking a train ticket
```

- Enter `8` → `Half price ticket`; `30` → `Full price ticket`; `70` → `Senior discount ticket`.
- Enter `Bus` / `Train` / `Flight` → the matching booking message; anything else → the `default` message.

---

## ☑️ Definition of done

- [ ] `Control_Structures` console project created and `Program.cs` replaced
- [ ] `TicketPricing()` uses an `if` / `else if` / `else` chain over `age`
- [ ] `TravelModeSelection()` uses a `switch` with `Bus` / `Train` / `Flight` cases **and** a `default`
- [ ] `Main` calls both routines in order
- [ ] `dotnet run` prints the correct price band and booking message for the inputs given

---

## 🔑 Key concepts

- **`if` / `else if` / `else` for ranges** — chained conditions test mutually exclusive
  age bands; order matters, so the first matching branch wins and the rest are skipped.
- **`switch` for discrete choices** — when you compare one value against a fixed set of
  options (the travel modes), a `switch` is clearer than a long `if-else` chain.
- **Always provide a `default`** — the fall-through case guards against invalid or
  unexpected input instead of silently doing nothing.
- **Validate and normalise input** — `int.TryParse` avoids a crash on non-numeric age,
  and `Trim().ToLower()` makes the `switch` tolerant of casing and stray spaces.
- **One responsibility per method** — splitting pricing and booking into `TicketPricing()`
  and `TravelModeSelection()` keeps `Main` readable and each decision testable on its own.
