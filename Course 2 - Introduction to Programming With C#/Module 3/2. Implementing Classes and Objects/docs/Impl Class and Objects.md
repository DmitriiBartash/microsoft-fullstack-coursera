# Implementing Classes and Objects

**Course 2 — Introduction to Programming With C#** · Module 3 · Lesson 2 · `You Try It!`

> Build a small C# console app that models a person with a **class**, then creates
> several **objects** from it. The point of the lab is the object-oriented core:
> a `Person` class holds state (`Name`, `Age`) and behavior (`Greet()`), and `Main`
> instantiates multiple independent objects that each carry their own data.

---

## 🎯 Objective

Practice defining a class with **properties** and a **method**, then **instantiating
objects** from it and calling that method on each — seeing how every object holds its
own state independently.

---

## 🗂️ What you will build

A console project named **`ImplClassAndObjects`** made of two files:

| File         | Responsibility                                                       |
| ------------ | -------------------------------------------------------------------- |
| `Person.cs`  | Define the `Person` class — `Name`, `Age` properties and `Greet()`   |
| `Program.cs` | Create three `Person` objects and call `Greet()` on each             |

**Flow:** `Program → new Person { … } → person.Greet() → personalized greeting on the console`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (the console application created at the start of the course)
- The C# Dev Kit / C# extension for Visual Studio Code

---

## 🛠️ Steps

### Step 1 — Prepare the application

Reuse the Visual Studio Code console application you created at the start of the course.
**Clear any existing code** in `Program.cs` so you can build the lab fresh in that file,
and add a new `Person.cs` file alongside it for the class.

```bash
dotnet new console -n ImplClassAndObjects
cd ImplClassAndObjects
```

### Step 2 — Create a basic class

In a new **`Person.cs`** file, define a class called `Person` with properties that
represent a person's characteristics.

- Define a class called `Person`.
- Create two properties: `Name` (a `string`) and `Age` (an `int`).

```csharp
namespace ImplClassAndObjects
{
    public class Person
    {
        public required string Name { get; set; }
        public int Age { get; set; }
    }
}
```

### Step 3 — Create objects

In `Program.cs`, create objects from the `Person` class and assign values to their
properties.

- Create two objects of the `Person` class.
- Assign different values to their `Name` and `Age` properties.

```csharp
using ImplClassAndObjects;

// Step 3: Create two Person objects
Person person1 = new()
{
    Name = "Alice",
    Age = 30
};
Person person2 = new()
{
    Name = "Bob",
    Age = 25
};
```

### Step 4 — Create a method

Add a method to the `Person` class that performs an action — printing a greeting.

- Write a method called `Greet` on the `Person` class.
- Make it print a message that includes the person's name.

```csharp
namespace ImplClassAndObjects
{
    public class Person
    {
        public required string Name { get; set; }
        public int Age { get; set; }

        public void Greet()
        {
            Console.WriteLine($"Hello, my name is {Name} and i am {Age} years old.");
        }
    }
}
```

### Step 5 — Use methods on objects

Call the `Greet` method on each `Person` object to perform an action.

- In `Main`, call `Greet` on each `Person` object to print a personalized greeting.

```csharp
// Step 5: Call Greet on each object
person1.Greet();
person2.Greet();
```

> To check your answer, run the Visual Studio Code console application. If you receive
> an error, go to the reading on the next page to compare your code to the correct answer.

### Step 6 — Practice with multiple objects

Create an additional object from the same class to see how each behaves independently.

- Create a third `Person` object.
- Assign it a different name and age.
- Call `Greet` on this new object.

```csharp
// Step 6: Create a third object and call Greet
Person person3 = new()
{
    Name = "Charlie",
    Age = 35
};
person3.Greet();
```

Run the application:

```bash
dotnet run
```

---

## ▶️ Expected result

Each object prints its own greeting, proving every `Person` carries independent state:

```text
Hello, my name is Alice and i am 30 years old.
Hello, my name is Bob and i am 25 years old.
Hello, my name is Charlie and i am 35 years old.
```

---

## ☑️ Definition of done

- [ ] `Person.cs` defines a `Person` class with `Name` (`string`) and `Age` (`int`) properties
- [ ] `Person` has a `Greet()` method that prints a message including the name
- [ ] `Program.cs` creates `person1` (Alice, 30) and `person2` (Bob, 25)
- [ ] `Greet()` is called on each object
- [ ] A third object `person3` (Charlie, 35) is added and `Greet()` is called on it
- [ ] `dotnet run` prints three independent personalized greetings without errors

---

## 🔑 Key concepts

- **Class vs. object** — `Person` is the blueprint (class); `person1`, `person2`, `person3`
  are concrete instances (objects) created from it with `new()`.
- **State lives per object** — each object stores its own `Name` and `Age`, so the same
  `Greet()` method produces a different result for each instance.
- **Properties** — auto-implemented `{ get; set; }` properties expose a person's data;
  `required string Name` forces a value to be supplied during object initialization.
- **Methods add behavior** — `Greet()` couples behavior with the data it acts on, the
  essence of encapsulation in object-oriented programming.
- **Object initializer syntax** — `new() { Name = "Alice", Age = 30 }` sets properties
  inline at creation, keeping construction concise and readable.
