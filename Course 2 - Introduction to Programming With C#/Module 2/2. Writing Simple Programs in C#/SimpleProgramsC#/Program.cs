namespace SimplePrograms
{
    class Calculator
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }

    class InputHelper
    {
        public static bool TryReadInteger(string prompt, out int number)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            return int.TryParse(input, out number);
        }

        public static void WaitForKey()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }

    class NumberDisplay
    {
        public static void DisplayNumbers()
        {
            Console.WriteLine("Numbers from 1 to 10:");
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine(i);
            }
        }
    }

    class UserInput
    {
        public static void GreetUser()
        {
            Console.Write("Enter your name: ");
            string? name = Console.ReadLine();

            Console.WriteLine("Hello, " + name + "!");
        }
    }

    class SimplePrograms
    {
        static void Main()
        {
            Console.WriteLine("--- Welcome to Simple Programs ---");
            UserInput.GreetUser();

            while (true)
            {
                Console.WriteLine("\n--- Main Menu ---");
                Console.WriteLine("1. Add two numbers");
                Console.WriteLine("2. Display numbers from 1 to 10");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();

                if (choice == "1")
                {
                    if (!InputHelper.TryReadInteger("Enter the first number: ", out int number1))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }

                    if (!InputHelper.TryReadInteger("Enter the second number: ", out int number2))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }

                    int result = Calculator.Add(number1, number2);
                    Console.WriteLine("The sum is: " + result);
                    InputHelper.WaitForKey();
                }
                else if (choice == "2")
                {
                    NumberDisplay.DisplayNumbers();
                    InputHelper.WaitForKey();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose 1, 2 or 3.");
                }
            }
        }
    }
}
