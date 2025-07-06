public class Program
{
    // Function to calculate the area of a circle
    static double CalculateCircleArea(double radius)
    {
        return Math.PI * radius * radius;
    }

    // Function to calculate the area of a trapezoid
    static double CalculateTrapezoidArea(double a, double b, double height)
    {
        return (a + b) / 2 * height;
    }

    public static void Main()
    {
        // PROBLEM 1: Circle Area
        Console.Write("Enter the radius of the circle: ");
        double radius = Convert.ToDouble(Console.ReadLine());
        double circleArea = CalculateCircleArea(radius);
        Console.WriteLine($"The area of the circle with radius {radius} is {circleArea:F2}");

        // PROBLEM 2: Trapezoid Area
        Console.Write("\nEnter the length of side a of the trapezoid: ");
        double a = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the length of side b of the trapezoid: ");
        double b = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the height of the trapezoid: ");
        double height = Convert.ToDouble(Console.ReadLine());
        double trapezoidArea = CalculateTrapezoidArea(a, b, height);
        Console.WriteLine($"The area of the trapezoid is {trapezoidArea:F2}");
    }
}
