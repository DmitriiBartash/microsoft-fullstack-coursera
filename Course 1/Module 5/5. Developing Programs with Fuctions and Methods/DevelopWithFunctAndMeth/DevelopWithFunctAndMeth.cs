namespace DevelopWithFunctAndMeth
{
    public class DevelopWithFunctAndMeth
    {
        public static void Main()
        {
            // Step 1: Welcome message
            DisplayWelcomeMessage();

            // Step 2: Greet user
            string name = GetUserName();
            GreetUser(name);

            // Step 3: Sum calculation
            int num1 = ReadInteger("Enter the first number: ");
            int num2 = ReadInteger("Enter the second number: ");
            int result = CalculateSum(num1, num2);
            Console.WriteLine($"The sum of {num1} and {num2} is {result}.");

            // Step 4: Check if a user-provided number is positive
            int numberToCheck = ReadInteger("Enter a number to check if it is positive: ");
            if (IsPositive(numberToCheck))
            {
                Console.WriteLine($"{numberToCheck} is a positive number.");
            }
            else
            {
                Console.WriteLine($"{numberToCheck} is not a positive number.");
            }

            // Step 5: Validate user age
            int age = ReadInteger("Please enter your age: ");
            if (IsOldEnoughToDrive(age))
            {
                Console.WriteLine("You are old enough to drive.");
            }
            else
            {
                Console.WriteLine("You are not old enough to drive.");
            }
        }

        public static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to the Program!");
        }

        public static string GetUserName()
        {
            Console.Write("Please enter your name: ");
            string? input = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Try again.");
                Console.Write("Please enter your name: ");
                input = Console.ReadLine();
            }

            return input;
        }

        public static void GreetUser(string name)
        {
            Console.WriteLine($"Hello, {name}!");
        }

        public static int ReadInteger(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            int number;

            while (!int.TryParse(input, out number))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }

            return number;
        }

        public static int CalculateSum(int x, int y) => x + y;

        public static bool IsPositive(int number)
        {
            return number > 0;
        }

        public static bool IsOldEnoughToDrive(int age)
        {
            return age >= 18;
        }
    }
}
