namespace Loop_Based
{
    public class Step1TotalScore
    {
        public static void Run()
        {
            int[] scores = [85, 90, 78, 92, 88];
            int totalScore = 0;

            for (int i = 0; i < scores.Length; i++)
            {
                totalScore += scores[i];
            }

            Console.WriteLine($"Total Score: {totalScore}");
        }
    }
}
