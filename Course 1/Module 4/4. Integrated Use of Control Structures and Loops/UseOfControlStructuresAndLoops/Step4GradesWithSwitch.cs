namespace UseOfControlStructuresAndLoops
{
    public class Step4GradesWithSwitch
    {
        public static void Run()
        {
            int[] scores = [95, 82, 76, 64, 58, 89, 70];

            for (int i = 0; i < scores.Length; i++)
            {
                int score = scores[i];
                string letterGrade;

                // Divide score by 10 to simplify switch logic
                switch (score / 10)
                {
                    case 10:
                    case 9:
                        letterGrade = "A";
                        break;
                    case 8:
                        letterGrade = "B";
                        break;
                    case 7:
                        letterGrade = "C";
                        break;
                    case 6:
                        letterGrade = "D";
                        break;
                    default:
                        letterGrade = "F";
                        break;
                }

                Console.WriteLine($"Score: {score} - Grade: {letterGrade}");
            }
        }
    }
}
