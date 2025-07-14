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
