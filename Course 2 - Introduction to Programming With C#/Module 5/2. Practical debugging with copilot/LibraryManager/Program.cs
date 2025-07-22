class LibraryManager
{
    static void Main()
    {
        string[] books = new string[5];

        while (true)
        {
            Console.WriteLine("Would you like to add or remove a book? (add/remove/exit)");
            string action = Console.ReadLine()?.Trim().ToLower();

            if (action == "add")
            {
                bool hasSpace = false;
                foreach (var book in books)
                {
                    if (string.IsNullOrEmpty(book))
                    {
                        hasSpace = true;
                        break;
                    }
                }

                if (!hasSpace)
                {
                    Console.WriteLine("The library is full. No more books can be added.");
                    continue;
                }

                Console.WriteLine("Enter the title of the book to add:");
                string newBook = Console.ReadLine()?.Trim();

                for (int i = 0; i < books.Length; i++)
                {
                    if (string.IsNullOrEmpty(books[i]))
                    {
                        books[i] = newBook;
                        Console.WriteLine($"Book \"{newBook}\" added.");
                        break;
                    }
                }
            }
            else if (action == "remove")
            {
                Console.WriteLine("Enter the title of the book to remove:");
                string removeBook = Console.ReadLine()?.Trim();

                bool found = false;
                for (int i = 0; i < books.Length; i++)
                {
                    if (books[i] == removeBook)
                    {
                        books[i] = "";
                        Console.WriteLine($"Book \"{removeBook}\" removed.");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else if (action == "exit")
            {
                Console.WriteLine("Exiting program...");
                break;
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'add', 'remove', or 'exit'.");
            }

            // Display the list of books
            Console.WriteLine("\nAvailable books:");
            foreach (var book in books)
            {
                if (!string.IsNullOrEmpty(book))
                {
                    Console.WriteLine($"- {book}");
                }
            }
            Console.WriteLine();
        }
    }
}
