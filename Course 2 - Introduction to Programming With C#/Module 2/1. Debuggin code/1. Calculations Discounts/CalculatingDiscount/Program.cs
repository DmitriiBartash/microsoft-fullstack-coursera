public class Program
{
    // Method to calculate the final price after applying a percentage discount
    public static double ApplyDiscount(double price, double discountPercentage)
    {
        // Error checks
        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }

        if (discountPercentage < 0 || discountPercentage > 100)
        {
            throw new ArgumentException("Discount percentage must be between 0 and 100.");
        }

        // Calculate the discount
        double discountAmount = price * (discountPercentage / 100);
        return price - discountAmount;
    }

    public static void Main()
    {
        try
        {
            double finalPrice = ApplyDiscount(1000, 15); // Valid input
            Console.WriteLine("The final price is: " + finalPrice);

            // Uncomment to test error handling:
            // double invalid = ApplyDiscount(-200, 10);   // Negative price
            // double invalid = ApplyDiscount(500, 150);   // Invalid discount percentage
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
