class Program
{
    static void Main()
    {
        List<string> tasks = new List<string>();
        bool exit = false;

        while (!exit)
        {
            ShowMenu();
            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        ViewTasks(tasks);
                        break;
                    case 2:
                        AddTask(tasks);
                        break;
                    case 3:
                        MarkTaskComplete(tasks);
                        break;
                    case 4:
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }

            Console.WriteLine(); 
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("=== TO-DO LIST MENU ===");
        Console.WriteLine("1. View Tasks");
        Console.WriteLine("2. Add Task");
        Console.WriteLine("3. Mark Task Complete");
        Console.WriteLine("4. Exit");
    }

    static void ViewTasks(List<string> tasks)
    {
        Console.WriteLine("\nYour Tasks:");

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
        }
        else
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i]}");
            }
        }
    }

    static void AddTask(List<string> tasks)
    {
        Console.Write("Enter the task: ");
        string? input = Console.ReadLine();
        string task = input != null ? input.Trim() : string.Empty;

        if (string.IsNullOrEmpty(task))
        {
            Console.WriteLine("Task cannot be empty.");
        }
        else
        {
            tasks.Add(task);
            Console.WriteLine("Task added.");
        }
    }

    static void MarkTaskComplete(List<string> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks to mark complete.");
            return;
        }

        Console.Write("Enter the task number to mark complete: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) &&
            taskNumber > 0 && taskNumber <= tasks.Count)
        {
            string currentTask = tasks[taskNumber - 1];
            if (currentTask.EndsWith(" [Complete]"))
            {
                Console.WriteLine("This task is already marked as complete.");
            }
            else
            {
                tasks[taskNumber - 1] += " [Complete]";
                Console.WriteLine("Task marked as complete.");
            }
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }
}
