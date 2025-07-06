namespace UseOfControlStructuresAndLoops
{
    public class Step2EvaluateGrades
    {
        public static void Run()
        {
            int[] grades = [85, 42, 73, 64, 90, 58, 67];

            for (int i = 0; i < grades.Length; i++)
            {
                int grade = grades[i];

                if (grade >= 65)
                {
                    Console.WriteLine("Grade: " + grade + " - Pass");
                }
                else
                {
                    Console.WriteLine("Grade: " + grade + " - Fail");
                }
            }
        }
    }
}
