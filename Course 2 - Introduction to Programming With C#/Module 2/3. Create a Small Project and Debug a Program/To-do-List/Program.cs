class ToDoList
{
    static readonly string[] tasks = new string[10];
    static int taskCount = 0;

    static void AddTask()
    {
        if (taskCount >= tasks.Length)
        {
            Console.WriteLine("⚠️ Task list is full.");
            return;
        }

        Console.Write("Enter a new task: ");
        string? task = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(task))
        {
            tasks[taskCount++] = task;
            Console.WriteLine("✅ Task added.");
        }
        else
        {
            Console.WriteLine("❌ Error: Task cannot be empty.");
        }
    }

    static void ViewTasks()
    {
        if (taskCount == 0)
        {
            Console.WriteLine("📭 No tasks to display.");
            return;
        }

        Console.WriteLine("\n📋 To-Do List:");
        for (int i = 0; i < taskCount; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i]}");
        }
    }

    static void CompleteTask()
    {
        Console.Write("Enter the number of the task to mark as completed: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int taskNumber))
        {
            Console.WriteLine($"🛠 DEBUG: You entered task number {taskNumber}");

            if (taskNumber < 1 || taskNumber > taskCount)
            {
                Console.WriteLine("❌ Error: Invalid task number.");
                return;
            }

            tasks[taskNumber - 1] += " [Completed]";
            Console.WriteLine("✅ Task marked as completed.");
        }
        else
        {
            Console.WriteLine("❌ Error: Please enter a valid number.");
        }
    }

    static void DeleteTask()
    {
        Console.Write("Enter the number of the task to delete: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int taskNumber))
        {
            if (taskNumber < 1 || taskNumber > taskCount)
            {
                Console.WriteLine("❌ Error: Invalid task number.");
                return;
            }

            // Shift tasks to fill the deleted spot
            for (int i = taskNumber - 1; i < taskCount - 1; i++)
            {
                tasks[i] = tasks[i + 1];
            }

            tasks[taskCount - 1] = null!;
            taskCount--;

            Console.WriteLine("🗑️ Task deleted.");
        }
        else
        {
            Console.WriteLine("❌ Error: Please enter a valid number.");
        }
    }

    static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n--- To-Do List Menu ---");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Complete Task");
            Console.WriteLine("4. Delete Task");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option (1-5): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ViewTasks();
                    break;
                case "3":
                    CompleteTask();
                    break;
                case "4":
                    DeleteTask();
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("👋 Exiting program...");
                    break;
                default:
                    Console.WriteLine("❌ Invalid option. Try again.");
                    break;
            }
        }
    }
}
