namespace Loop_Based
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Task 1: Total Score");
            Step1TotalScore.Run();

            Console.WriteLine("\nRunning Task 2: Factorial");
            Step2CalcFactorials.Run();

            Console.WriteLine("\nRunning Task 3: Pass or Fail");
            Step3PassOrFail.Run();

            Console.WriteLine("\nRunning Task 4: Weekly Task Scheduler");
            Step4ComboLoopsSwitch.Run();
        }
    }
}
