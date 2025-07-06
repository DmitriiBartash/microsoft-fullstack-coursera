class Program
{
    static void Main()
    {
        Console.Write("Enter the radius of the circle: ");
        string? input = Console.ReadLine();

        // Convert string to double
        if (double.TryParse(input, out double radius))
        {
            double area = CalculateCircleArea(radius);
            Console.WriteLine($"The area of the circle is: {area:F2}");
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
        }


        Console.Write("Enter the length of side a: ");
        string? inputA = Console.ReadLine();

        Console.Write("Enter the length of side b: ");
        string? inputB = Console.ReadLine();

        Console.Write("Enter the height: ");
        string? inputHeight = Console.ReadLine();

        // Parse inputs to double
        if (double.TryParse(inputA, out double a) &&
            double.TryParse(inputB, out double b) &&
            double.TryParse(inputHeight, out double height))
        {
            double area = CalculateTrapezoidArea(a, b, height);
            Console.WriteLine($"The area of the trapezoid is: {area:F2}");
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter numeric values.");
        }
    }

    // Method to calculate area
    static double CalculateCircleArea(double radius)
    {
        return Math.PI * radius * radius;
    }

    // Method to calculate trapezoid area
    static double CalculateTrapezoidArea(double a, double b, double height)
    {
        return (a + b) / 2 * height; 
    }
}