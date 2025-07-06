namespace MembershipFeeCalcSys
{
    public class MembershipFeeCalcSysOpt
    {
        // Static readonly for better performance than const for complex types
        private static readonly Dictionary<MembershipType, decimal> BaseFees = new()
        {
            { MembershipType.Basic, 50m },
            { MembershipType.Premium, 100m },
            { MembershipType.VIP, 200m }
        };

        private static readonly Dictionary<int, decimal> DurationDiscounts = new()
        {
            { 12, 0.10m },  // 10% for 1 year
            { 24, 0.20m },  // 20% for 2 years
            { 36, 0.30m }   // 30% for 3 years
        };

        // Optimized calculation method using switch expression (C# 8.0+)
        public decimal CalculateFee(MembershipType type, int durationMonths, bool isStudent = false, int age = 0)
        {
            // Input validation with early return
            if (durationMonths <= 0) return 0;

            var baseFee = BaseFees.GetValueOrDefault(type, 0);
            if (baseFee == 0) return 0;

            var totalFee = baseFee * durationMonths;

            // Apply duration discount using optimized lookup
            var discount = GetOptimalDiscount(durationMonths);
            totalFee *= (1 - discount);

            // Apply additional discounts using pattern matching
            var additionalDiscount = (isStudent, age) switch
            {
                (true, >= 18 and <= 25) => 0.15m,  // Student discount
                (_, >= 65) => 0.10m,                // Senior discount
                _ => 0m
            };

            return Math.Round(totalFee * (1 - additionalDiscount), 2);
        }

        // Optimized discount calculation with binary search concept
        private static decimal GetOptimalDiscount(int months)
        {
            return months switch
            {
                >= 36 => DurationDiscounts[36],
                >= 24 => DurationDiscounts[24],
                >= 12 => DurationDiscounts[12],
                _ => 0m
            };
        }

        // Batch processing for multiple members (performance optimization)
        public Dictionary<int, decimal> CalculateFeesForMembers(
            IEnumerable<(int Id, MembershipType Type, int Duration, bool IsStudent, int Age)> members)
        {
            return members.AsParallel()
                         .ToDictionary(
                             member => member.Id,
                             member => CalculateFee(member.Type, member.Duration, member.IsStudent, member.Age)
                         );
        }

        // Memory-efficient method for fee comparison
        public (decimal Basic, decimal Premium, decimal VIP) GetAllMembershipFees(int duration, bool isStudent = false, int age = 0)
        {
            return (
                CalculateFee(MembershipType.Basic, duration, isStudent, age),
                CalculateFee(MembershipType.Premium, duration, isStudent, age),
                CalculateFee(MembershipType.VIP, duration, isStudent, age)
            );
        }
    }

    public enum MembershipType : byte  // Using byte for memory efficiency
    {
        Basic = 1,
        Premium = 2,
        VIP = 3
    }
}