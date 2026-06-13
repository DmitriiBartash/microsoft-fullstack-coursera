# Inheritance and Polymorphism

**Course 2 — Introduction to Programming With C#** · Module 3 · Lesson 1 · `You Try It!`

> Build a small C# console app that models an `Animal` base class with `Dog` and `Cat`
> derived classes. Along the way you'll practise the four pillars of OOP reuse in C#:
> **inheritance**, the **`virtual`/`override`** keyword pair, **interfaces**, and
> **polymorphism** — calling one method on many types through a shared base type.

---

## 🎯 Objective

Use a single console application to see how `Dog` and `Cat` can **inherit** from a common
`Animal` base class, **override** its behaviour, satisfy an `IAnimal` **interface contract**,
and finally be driven **polymorphically** through a `List<Animal>` so one loop speaks to every
animal type.

---

## 🗂️ What you will build

A single console project. All of the code lives in **`Program.cs`** and is grouped into a few
small types:

| Type            | Kind             | Responsibility                                             |
| --------------- | ---------------- | ---------------------------------------------------------- |
| `IAnimal`       | interface        | Declares the `Eat()` contract every animal must fulfil     |
| `Animal`        | base class       | Default `MakeSound()` and `Eat()`, both marked `virtual`   |
| `Dog`           | derived class    | `override`s `MakeSound()` and `Eat()` with dog behaviour   |
| `Cat`           | derived class    | `override`s `MakeSound()` and `Eat()` with cat behaviour   |
| `Program`       | entry point      | `Main()` creates instances and exercises them              |

**Flow:** `Animal` (virtual) → `Dog` / `Cat` (override) → `List<Animal>` → `foreach` → `MakeSound()`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (the console app you created at the start of the course)
- The `Program.cs` file of that console application, ready to be cleared

---

## 🛠️ Steps

### Step 1 — Prepare the application

Reuse the Visual Studio Code console application you created at the start of the course. Open its
`Program.cs`, **remove any existing code**, and build every step below in that one file.

> All five remaining steps add code to the **same** `Program.cs`.

### Step 2 — Create a base class and derived classes

Create a base class called `Animal` and two derived classes, `Dog` and `Cat`. This is the heart of
**inheritance**: `Dog` and `Cat` automatically receive the members declared on `Animal`.

- Define a base class `Animal` with a `virtual` method `MakeSound`.
- Create two derived classes `Dog` and `Cat` that inherit from `Animal` (`class Dog : Animal`).
- Override the `MakeSound` method in each derived class.

```csharp
class Animal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("The animal makes a sound.");
    }
}

class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("The dog barks: Woof!");
    }
}

class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("The cat meows: Meow!");
    }
}
```

### Step 3 — Use the `virtual` and `override` keywords

Explore how the base method you marked `virtual` is replaced by the `override` versions at runtime.

- Above any existing classes in `Program.cs`, create a `Program` class.
- In the `Program` class, create a `Main` method.
- In `Main`, create instances of `Dog` and `Cat`, then call `MakeSound` on each.

```csharp
class Program
{
    static void Main(string[] args)
    {
        Dog myDog = new();
        Cat myCat = new();

        myDog.MakeSound(); // The dog barks: Woof!
        myCat.MakeSound(); // The cat meows: Meow!
    }
}
```

> To check your answer, run the Visual Studio Code console application with `dotnet run`. If you
> get an error, compare your code against the worked solution at the end of this lab.

### Step 4 — Implement an interface

Introduce an **interface** to define a contract that classes must implement. An interface specifies
a set of methods that different classes are required to provide.

- Above any existing classes in `Program.cs`, define an interface called `IAnimal` with a method `Eat`.
- Implement this interface in the `Animal` class (`class Animal : IAnimal`) and provide an
  implementation of `Eat` in the `Dog` and `Cat` classes.

```csharp
interface IAnimal
{
    void Eat();
}

class Animal : IAnimal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("The animal makes a sound.");
    }

    public virtual void Eat()
    {
        Console.WriteLine("The animal eats food.");
    }
}
```

```csharp
class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("The dog barks: Woof!");
    }

    public override void Eat()
    {
        Console.WriteLine("The dog eats bones.");
    }
}

class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("The cat meows: Meow!");
    }

    public override void Eat()
    {
        Console.WriteLine("The cat eats fish.");
    }
}
```

> Because `Animal` satisfies `IAnimal`, every class that derives from `Animal` is also an `IAnimal`.

### Step 5 — Use the interface

Call the interface method from your main program.

- In `Main`, underneath the existing method calls, call the `Eat` method on the `Dog` and `Cat` instances.

```csharp
        myDog.Eat(); // The dog eats bones.
        myCat.Eat(); // The cat eats fish.
```

> Run the app again with `dotnet run` to confirm. If you get an error, check your code against the
> worked solution below.

### Step 6 — Polymorphism with lists of base types and interfaces

Use **polymorphism** to interact with objects of different classes through a common base type. This
lets one loop call `MakeSound` on every object without knowing its concrete type.

- Update `Main` by creating a `List<Animal>` that includes instances of `Dog` and `Cat`.
- Use a loop to call `MakeSound` on each object in the list.

```csharp
        List<Animal> animals =
        [
            new Dog(),
            new Cat()
        ];

        Console.WriteLine("\nPolymorphic behavior using base class reference:");
        foreach (Animal animal in animals)
        {
            animal.MakeSound();
        }
```

> The variable is typed `Animal`, but the *runtime type* (`Dog` or `Cat`) decides which overridden
> `MakeSound` actually runs. Run with `dotnet run`; if it errors, compare against the solution below.

### Worked solution — complete `Program.cs`

Putting all six steps together, the finished file looks like this:

```csharp
// Step 4.1: Define an interface IAnimal with a method Eat
interface IAnimal
{
    void Eat();
}

// Step 2.1 + Step 4.2: Base class Animal implements IAnimal
class Animal : IAnimal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("The animal makes a sound.");
    }

    public virtual void Eat()
    {
        Console.WriteLine("The animal eats food.");
    }
}

// Step 2.2 + 2.3 + Step 4.2: Derived class Dog
class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("The dog barks: Woof!");
    }

    public override void Eat()
    {
        Console.WriteLine("The dog eats bones.");
    }
}

// Step 2.2 + 2.3 + Step 4.2: Derived class Cat
class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("The cat meows: Meow!");
    }

    public override void Eat()
    {
        Console.WriteLine("The cat eats fish.");
    }
}

// Step 3 + Step 5 + Step 6: Program class
class Program
{
    static void Main(string[] args)
    {
        // Step 3: Create Dog and Cat instances and call MakeSound
        Dog myDog = new();
        Cat myCat = new();
        myDog.MakeSound(); // The dog barks: Woof!
        myCat.MakeSound(); // The cat meows: Meow!

        // Step 5.1: Call Eat method from Dog and Cat instances
        myDog.Eat();       // The dog eats bones.
        myCat.Eat();       // The cat eats fish.

        // Step 6.1: Create a list of Animal objects
        List<Animal> animals =
        [
            new Dog(),
            new Cat()
        ];

        // Step 6.2: Use a loop to call MakeSound
        Console.WriteLine("\nPolymorphic behavior using base class reference:");
        foreach (Animal animal in animals)
        {
            animal.MakeSound();
        }
    }
}
```

---

## ▶️ Expected result

Running `dotnet run` prints the direct calls, then the interface calls, then the polymorphic loop:

```text
The dog barks: Woof!
The cat meows: Meow!
The dog eats bones.
The cat eats fish.

Polymorphic behavior using base class reference:
The dog barks: Woof!
The cat meows: Meow!
```

---

## ☑️ Definition of done

- [ ] `Program.cs` cleared and rebuilt from the steps above
- [ ] Base class `Animal` defines `virtual` methods `MakeSound` and `Eat`
- [ ] `Dog` and `Cat` derive from `Animal` and `override` both methods
- [ ] Interface `IAnimal` declares `Eat`, and `Animal` implements `IAnimal`
- [ ] `Main` creates `Dog`/`Cat` instances and calls `MakeSound` and `Eat`
- [ ] A `List<Animal>` is iterated with `foreach` to call `MakeSound` polymorphically
- [ ] `dotnet run` produces the expected output above

---

## 🔑 Key concepts

- **Inheritance** — `Dog` and `Cat` reuse the members of `Animal` via `class Dog : Animal`, so shared
  behaviour is written once on the base class.
- **`virtual` + `override`** — marking a base method `virtual` *allows* a subclass to replace it; the
  matching `override` supplies the specialised version that runs for that type.
- **Interfaces as contracts** — `IAnimal` says "every animal must `Eat()`" without dictating *how*;
  implementing it on `Animal` propagates the contract to all derived classes.
- **Polymorphism** — a `List<Animal>` can hold any subclass, and the loop calls the *runtime* type's
  overridden method, so one piece of code drives many behaviours.
- **Base-type references** — programming to `Animal` (the abstraction) rather than `Dog`/`Cat` (the
  concretions) keeps calling code open to new animal types without changes.
