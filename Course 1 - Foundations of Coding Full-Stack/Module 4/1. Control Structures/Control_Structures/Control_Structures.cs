namespace Control_Structures
{
    class Program
    {
        static void Main()
        {
            // Step 1: If-else Statement for Ticket Pricing
            Console.WriteLine("=== Ticket Pricing Program ===");
            TicketPricing();
            
            // Step 2:
            Console.WriteLine("\n=== Second Program ===");
            TravelModeSelection();
        }
        
        static void TicketPricing()
        {
            Console.Write("Please enter your age: ");
            string? input = Console.ReadLine();
            
            if (!int.TryParse(input, out int age))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }
            
            if (age < 0)
            {
                Console.WriteLine("Age cannot be negative.");
                return;
            }
            
            if (age < 12)
            {
                Console.WriteLine("Half price ticket");
            }
            else if (age < 65)
            {
                Console.WriteLine("Full price ticket");
            }
            else
            {
                Console.WriteLine("Senior discount ticket");
            }
        }

        static void TravelModeSelection()
        {
            Console.Write("Choose travel mode (Bus, Train, Flight): ");
            string? mode = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(mode))
            {
            Console.WriteLine("Input cannot be empty. Please enter a valid travel mode.");
            return;
            }

            switch (mode.Trim().ToLower())
            {
            case "bus":
                Console.WriteLine("Booking a bus ticket");
                break;
            case "train":
                Console.WriteLine("Booking a train ticket");
                break;
            case "flight":
                Console.WriteLine("Booking a flight ticket");
                break;
            default:
                Console.WriteLine("Invalid selection. Please choose Bus, Train, or Flight.");
                break;
            }
        }
    }
}