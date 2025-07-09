public class Program
{
    // Method to find the maximum number in an array
    public static int FindMax(int[] numbers)
    {
        // Error check 1: array is null
        if (numbers == null)
        {
            throw new ArgumentNullException(nameof(numbers), "Input array cannot be null.");
        }

        // Error check 2: array is empty
        if (numbers.Length == 0)
        {
            throw new ArgumentException("The array cannot be empty.");
        }

        // Initialize max to the first element
        int max = numbers[0];

        // Loop through the array starting from the second element
        for (int i = 1; i < numbers.Length; i++)
        {
            // Error check 3: check for int.MinValue overflow 
            if (numbers[i] < int.MinValue || numbers[i] > int.MaxValue)
            {
                throw new OverflowException("Array contains an out-of-range integer.");
            }

            if (numbers[i] > max)
            {
                max = numbers[i];
            }
        }

        return max;
    }

    public static void Main()
    {
        try
        {
            // Test array
            int[] myNumbers = { -5, -10, -3, -8, -2 };

            // Uncomment the following lines one at a time to test different errors:
            // int[] myNumbers = null;             // Triggers ArgumentNullException
            // int[] myNumbers = { };              // Triggers ArgumentException
            // int[] myNumbers = new int[1000000]; // Simulated size check (if needed)

            int maxNumber = FindMax(myNumbers);
            Console.WriteLine("The maximum number is: " + maxNumber);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine("Null Error: " + ex.Message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Argument Error: " + ex.Message);
        }
        catch (OverflowException ex)
        {
            Console.WriteLine("Overflow Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected Error: " + ex.Message);
        }
    }
}
