namespace Loop_Based
{
    public class Step4ComboLoopsSwitch
    {
        public static void Run()
        {
            string[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

            for (int i = 0; i < weekDays.Length; i++)
            {
                string day = weekDays[i];

                Console.Write($"{day}: ");

                switch (day)
                {
                    case "Monday":
                        Console.WriteLine("Team Meeting");
                        break;
                    case "Tuesday":
                        Console.WriteLine("Code Review");
                        break;
                    case "Wednesday":
                        Console.WriteLine("Development");
                        break;
                    case "Thursday":
                        Console.WriteLine("Testing");
                        break;
                    case "Friday":
                        Console.WriteLine("Deployment");
                        break;
                    default:
                        Console.WriteLine("No task assigned.");
                        break;
                }
            }
        }
    }
}
