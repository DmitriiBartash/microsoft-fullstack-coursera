namespace Loop_Based
{
    public class Step2CalcFactorials
    {
        public static void Run()
        {
            int number = 5;
            int factorial = 1;

            while (number > 0)
            {
                factorial *= number;
                number--;
            }

            Console.WriteLine($"Factorial: {factorial}");
        }
    }
}
