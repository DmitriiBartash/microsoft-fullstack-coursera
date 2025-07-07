using System;

class Program
{
    static void Main(string[] args)
    {
        // 1.1 part
        int sum1 = 0;
        int[] array = [1, 2, 3, 4, 5];

        foreach (int number in array)
        {
            sum1 += number;
        }

        Console.WriteLine(sum1);

        // 2.1 part
        int vowelCount1 = 0;
        char[] letters = ['h', 'e', 'l', 'l', 'o'];
        char[] vowels = ['a', 'e', 'i', 'o', 'u'];
        foreach (char letter in letters)
        {
            if (vowels.Contains(char.ToLower(letter)))
            {
                vowelCount1 += 1;
            }
        }

        Console.WriteLine(vowelCount1);

        // 1.2 part ideal
        // Initialize an array of integers
        int[] numbers = { 5, 8, 3, 4, 2 };

        // Initialize the sum variable
        int sum2 = 0;

        // Loop through each number in the array
        foreach (int number in numbers)
        {
            sum2 += number;
        }

        // Output the sum of the array
        Console.WriteLine("The sum of the array is: " + sum2);



        // 2.2 part ideal
        // Input string
        string input = "Hello, World!";
        // Initialize vowel count
        int vowelCount2 = 0;

        // Convert string to lowercase for easier comparison
        input = input.ToLower();

        // Loop through each character in the string
        foreach (char c in input)
        {
            // Check if the character is a vowel
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u')
            {
                vowelCount2++;
            }
        }

        // Output the number of vowels
        Console.WriteLine("Number of vowels: " + vowelCount2);

        // 3.1 part — Sum of user input numbers
        Console.WriteLine("Enter numbers separated by spaces:");
        string? userInputNumbers = Console.ReadLine();
        if (string.IsNullOrEmpty(userInputNumbers))
        {
            Console.WriteLine("No input provided.");
            return;
        }
        string[] parts = userInputNumbers.Split(' ');
        int sum3 = 0;

        foreach (string part in parts)
        {
            if (int.TryParse(part, out int num))
            {
                sum3 += num;
            }
        }

        Console.WriteLine("Sum of input numbers: " + sum3);

        // 3.2 part — Count vowels in user input string
        Console.WriteLine("Enter a string:");
        string? userInputText = Console.ReadLine();
        int vowelCount3 = 0;
        if (!string.IsNullOrEmpty(userInputText))
        {
            userInputText = userInputText.ToLower();

            foreach (char c in userInputText)
            {
                if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u')
                {
                    vowelCount3++;
                }
            }
        }

        Console.WriteLine("Number of vowels in input string: " + vowelCount3);

    }
}
