namespace MembershipFeeCalcSys
{
    public class Program
    {
        public static void Main()
        {
            // Step 1: Input age
            Console.WriteLine("Enter your age:");
            string? ageInput = Console.ReadLine();

            if (!int.TryParse(ageInput, out int age) || age < 0)
            {
                Console.WriteLine("Invalid age input");
                return;
            }

            // Step 2: 
            Console.WriteLine("Enter membership type (basic/premium):");
            string? membershipInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(membershipInput))
            {
                Console.WriteLine("Invalid membership type.");
                return;
            }

            string membershipType = membershipInput.Trim().ToLower();
            double fee = 0;

            if (age < 18)
            {
                if (membershipType == "basic")
                {
                    fee = 15;
                }
                else if (membershipType == "premium")
                {
                    fee = 25;
                }
                else
                {
                    Console.WriteLine("Unknown membership type.");
                    return;
                }
            }
            else if (age >= 18 && age <= 60)
            {
                if (membershipType == "basic")
                {
                    fee = 30;
                }
                else if (membershipType == "premium")
                {
                    fee = 50;
                }
                else
                {
                    Console.WriteLine("Unknown membership type.");
                    return;
                }
            }
            else if (age > 60) 
            {
                if (membershipType == "basic")
                {
                    fee = 20;
                }
                else if (membershipType == "premium")
                {
                    fee = 35;
                }
                else
                {
                    Console.WriteLine("Unknown membership type.");
                    return;
                }
            }
            // Step 4: Output the result
            Console.WriteLine($"The membership fee is: ${fee}");

        }
    }
}