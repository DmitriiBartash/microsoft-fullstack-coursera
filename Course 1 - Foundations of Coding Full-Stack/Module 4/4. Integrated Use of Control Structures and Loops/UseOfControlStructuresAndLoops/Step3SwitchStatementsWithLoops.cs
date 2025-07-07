namespace UseOfControlStructuresAndLoops
{
    public class Step3SwitchWithLoops
    {
        public static void Run()
        {
            string[] orderStatuses = ["Pending", "Shipped", "Delivered", "Cancelled", "Unknown"];

            foreach (string status in orderStatuses)
            {
                switch (status)
                {
                    case "Pending":
                        Console.WriteLine("Order is pending and will be processed soon.");
                        break;
                    case "Shipped":
                        Console.WriteLine("Order has been shipped.");
                        break;
                    case "Delivered":
                        Console.WriteLine("Order was delivered successfully.");
                        break;
                    case "Cancelled":
                        Console.WriteLine("Order has been cancelled.");
                        break;
                    default:
                        Console.WriteLine("Unknown order status: " + status);
                        break;
                }
            }
        }
    }
}
