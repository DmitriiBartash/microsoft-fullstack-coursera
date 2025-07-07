namespace ParamsInMethods
{
    public class ParamsInMethods
    {
        public static void Main()
        {
            // Problem 1
            int length = ReadAndValidate("Enter the length: ");
            int width = ReadAndValidate("Enter the width: ");
            int height = ReadAndValidate("Enter the height: ");

            int volume = VolumeOfRectangle(length, width, height);
            Console.WriteLine($"The volume of the rectangular box is: {volume}");

            // Problem 2
            int num1 = ReadAndValidate("Enter the first number: ");
            int num2 = ReadAndValidate("Enter the second number: ");
            int num3 = ReadAndValidate("Enter the third number: ");

            int average = CalculateAverage(num1, num2, num3);
            Console.WriteLine($"The average of the three numbers is: {average}");
        }

        public static int ReadAndValidate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input is empty or whitespace.");
                    continue;
                }

                if (!int.TryParse(input, out int number) || number <= 0)
                {
                    Console.WriteLine("Input must be a valid integer greater than 0.");
                    continue;
                }

                return number;
            }
        }

        public static int VolumeOfRectangle(int length, int width, int height)
        {
            return length * width * height;
        }

        public static int CalculateAverage(int num1, int num2, int num3)
        {
            return (num1 + num2 + num3) / 3;
        }
    }
}
