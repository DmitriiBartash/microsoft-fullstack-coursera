namespace LibraryManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            // Step 1: Create 5 string variables for book storage
            string book1 = "";
            string book2 = "";
            string book3 = "";
            string book4 = "";
            string book5 = "";

            while (true)
            {
                Console.WriteLine("\nChoose an action: add / remove / display / exit");
                string action = Console.ReadLine().Trim().ToLower();

                if (action == "add")
                {
                    // Check if there is any empty slot
                    if (book1 != "" && book2 != "" && book3 != "" && book4 != "" && book5 != "")
                    {
                        Console.WriteLine("Library is full. Cannot add more books.");
                        continue;
                    }

                    Console.Write("Enter the title of the book to add: ");
                    string newBook = Console.ReadLine();

                    if (book1 == "") book1 = newBook;
                    else if (book2 == "") book2 = newBook;
                    else if (book3 == "") book3 = newBook;
                    else if (book4 == "") book4 = newBook;
                    else if (book5 == "") book5 = newBook;

                    Console.WriteLine($"Book \"{newBook}\" added.");
                }
                else if (action == "remove")
                {
                    // Check if there's at least one book to remove
                    if (book1 == "" && book2 == "" && book3 == "" && book4 == "" && book5 == "")
                    {
                        Console.WriteLine("Library is empty. Nothing to remove.");
                        continue;
                    }

                    Console.Write("Enter the title of the book to remove: ");
                    string removeBook = Console.ReadLine();

                    if (book1 == removeBook) { book1 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book2 == removeBook) { book2 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book3 == removeBook) { book3 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book4 == removeBook) { book4 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else if (book5 == removeBook) { book5 = ""; Console.WriteLine($"Book \"{removeBook}\" removed."); }
                    else
                    {
                        Console.WriteLine("Book not found.");
                    }
                }
                else if (action == "display")
                {
                    Console.WriteLine("\nBooks in Library:");
                    if (book1 != "") Console.WriteLine("- " + book1);
                    if (book2 != "") Console.WriteLine("- " + book2);
                    if (book3 != "") Console.WriteLine("- " + book3);
                    if (book4 != "") Console.WriteLine("- " + book4);
                    if (book5 != "") Console.WriteLine("- " + book5);
                    if (book1 == "" && book2 == "" && book3 == "" && book4 == "" && book5 == "")
                        Console.WriteLine("(No books available)");
                }
                else if (action == "exit")
                {
                    Console.WriteLine("Exiting the Library Management System.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid action. Please type add, remove, display, or exit.");
                }
            }
        }
    }
}
