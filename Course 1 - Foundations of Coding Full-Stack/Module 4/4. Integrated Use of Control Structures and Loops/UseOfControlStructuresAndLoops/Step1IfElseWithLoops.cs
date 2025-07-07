namespace UseOfControlStructuresAndLoops
{
    public class Step1IfElseWithLoops
    {
        public static void Run()
        {
            do
            {
                Console.Write("Enter an even number between 1 and 10: ");
                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int number))
                {
                    if (number >= 1 && number <= 10 && number % 2 == 0)
                    {
                        Console.WriteLine("Valid input: " + number);
                        break; // Exit loop if valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. The number must be even and between 1 and 10.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a numeric value.");
                }

            } while (true);
        }
    }
}
