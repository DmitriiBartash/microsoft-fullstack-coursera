using ImplClassAndObjects;

class Program
{
    static void Main(string[] args)
    {
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

        // Step 5: Call Greet on each object
        person1.Greet();
        person2.Greet();

        // Step 6: Create a third object and call Greet
        Person person3 = new()
        {
            Name = "Charlie",
            Age = 35
        };
        person3.Greet();

    }
}