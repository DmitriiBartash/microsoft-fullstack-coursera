class TaskManager
{
    static readonly List<string> tasks = [];
    static readonly List<bool> taskStatus = [];

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n=== Task Manager ===");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Completed");
            Console.WriteLine("3. View Tasks");
            Console.WriteLine("4. Exit");
            Console.Write("What would you like to do? (choose 1-4): ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    CompleteTask();
                    break;
                case "3":
                    ViewTasks();
                    break;
                case "4":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                    break;
            }
        }
    }

    static void AddTask()
    {
        Console.Write("Enter task description: ");
        string task = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrEmpty(task))
        {
            Console.WriteLine("Task description cannot be empty.");
            return;
        }

        tasks.Add(task);
        taskStatus.Add(false);
        Console.WriteLine("Task added successfully.");
    }

    static void CompleteTask()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available to mark as completed.");
            return;
        }

        ViewTasks();
        Console.Write("Enter task number to mark as completed: ");
        string input = Console.ReadLine() ?? "";

        if (int.TryParse(input, out int taskNumber) && taskNumber >= 1 && taskNumber <= tasks.Count)
        {
            taskStatus[taskNumber - 1] = true;
            Console.WriteLine($"Task '{tasks[taskNumber - 1]}' marked as completed.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }

    static void ViewTasks()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
            return;
        }

        Console.WriteLine("\n--- Task List ---");
        for (int i = 0; i < tasks.Count; i++)
        {
            string status = taskStatus[i] ? "Completed" : "Pending";
            Console.WriteLine($"{i + 1}. {tasks[i]} - {status}");
        }
    }
}
