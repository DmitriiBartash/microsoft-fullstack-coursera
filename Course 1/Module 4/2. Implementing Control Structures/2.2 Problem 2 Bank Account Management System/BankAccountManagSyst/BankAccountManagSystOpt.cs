namespace BankAccountManagSyst
{
    public class BankAccountManagSystOpt
    {
        public static string ProcessAccount(string accountType, double initialBalance)
        {
            var account = new BankAccount(accountType.Trim().ToLower(), initialBalance);

            if (!account.ApplyRules())
                return "Unknown account type. Please enter savings, checking, or business.";

            return account.GetSummary();
        }
    }

    public class BankAccount
    {
        private string AccountType { get; }
        private double Balance { get; set; }

        public BankAccount(string accountType, double initialBalance)
        {
            AccountType = accountType;
            Balance = initialBalance;
        }

        public bool ApplyRules()
        {
            switch (AccountType)
            {
                case "savings":
                    AddInterest(0.02);
                    break;
                case "checking":
                    DeductFee(10);
                    break;
                case "business":
                    AddInterest(0.01);
                    DeductFee(20);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void AddInterest(double rate)
        {
            Balance += Balance * rate;
        }

        private void DeductFee(double fee)
        {
            Balance -= fee;
        }

        public string GetSummary()
        {
            return $"Account type: {AccountType}\nFinal balance: ${Balance:F2}";
        }
    }
}
