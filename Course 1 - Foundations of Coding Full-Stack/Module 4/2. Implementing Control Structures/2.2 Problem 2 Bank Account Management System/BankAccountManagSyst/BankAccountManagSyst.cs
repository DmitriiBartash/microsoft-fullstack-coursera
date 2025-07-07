namespace BankAccountManagSyst
{
    public class BankAccountManagSyst
    {
        public static void Main()
        {
            // Step 1: Get user input for account type
            Console.WriteLine("Enter account type (savings/checking/business):");
            string? accountInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(accountInput))
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            string accountType = accountInput.Trim().ToLower();

            double balance = 10000.00; // Example for demonstration
            double interestRate = 0, monthlyFee = 0;

            // Step 2: Use switch-case to determine behavior
            switch (accountInput)
            {
                case "savings":
                    interestRate = 0.02; // 2% interest
                    double interest = balance * interestRate;
                    balance += interest;
                    Console.WriteLine($"Savings account: +2% interest applied. New balance: ${balance:F2}");
                    break;

                case "checking":
                    monthlyFee = 10.0;
                    balance -= monthlyFee;
                    Console.WriteLine($"Checking account: $10 monthly fee deducted. New balance: ${balance:F2}");
                    break;

                case "business":
                    interestRate = 0.01; // 1% interest
                    monthlyFee = 20.0;
                    balance += balance * interestRate;
                    balance -= monthlyFee;
                    Console.WriteLine($"Business account: +1% interest and $20 fee applied. New balance: ${balance:F2}");
                    break;

                default: 
                    Console.WriteLine("Unknown account type. Please enter savings, checking, or business.");
                    break;

            }
        }
    }
}