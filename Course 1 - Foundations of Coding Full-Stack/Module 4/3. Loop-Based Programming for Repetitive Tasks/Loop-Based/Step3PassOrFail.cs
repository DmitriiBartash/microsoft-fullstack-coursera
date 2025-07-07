namespace Loop_Based
{
    public class Step3PassOrFail
    {
        public static void Run()
        {
            int[] studentScores = { 45, 60, 72, 38, 55 };

            for (int i = 0; i < studentScores.Length; i++)
            {
                if (studentScores[i] >= 50)
                {
                    Console.WriteLine($"Score: {studentScores[i]} - Pass");
                }
                else
                {
                    Console.WriteLine($"Score: {studentScores[i]} - Fail");
                }
            }
        }
    }
}
