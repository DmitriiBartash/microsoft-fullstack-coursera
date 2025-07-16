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