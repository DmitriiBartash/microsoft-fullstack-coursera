using System;

namespace TaskManager
{
    class TaskManager
    {
        // Task descriptions and statuses
        static string task1 = "", task2 = "", task3 = "";
        static bool isTask1Completed = false, isTask2Completed = false, isTask3Completed = false;

        static void Main()
        {
            bool running = true;

            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        MarkTaskAsCompleted();
                        break;
                    case "3":
                        ShowTasks();
                        break;
                    case "4":
                        running = false;
                        Console.WriteLine("Exiting Task Manager. Goodbye!");
                        break;
                    case "5":
                        RemoveTask();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n=== Task Manager ===");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Completed");
            Console.WriteLine("3. Show Tasks");
            Console.WriteLine("4. Exit");
            Console.WriteLine("5. Remove (Clean) a Task");
            Console.Write("Choose an option (1-5): ");
        }

        static void AddTask()
        {
            if (task1 == "")
            {
                task1 = GetTaskDescription();
                Console.WriteLine("Task 1 added.");
            }
            else if (task2 == "")
            {
                task2 = GetTaskDescription();
                Console.WriteLine("Task 2 added.");
            }
            else if (task3 == "")
            {
                task3 = GetTaskDescription();
                Console.WriteLine("Task 3 added.");
            }
            else
            {
                Console.WriteLine("Task list is full. You can't add more than 3 tasks.");
            }
        }

        static string GetTaskDescription()
        {
            Console.Write("Enter task description: ");
            return Console.ReadLine() ?? "";
        }

        static void MarkTaskAsCompleted()
        {
            Console.Write("Enter task number to mark as completed (1-3): ");
            string taskNum = Console.ReadLine() ?? "";

            if (taskNum == "1" && task1 != "")
                isTask1Completed = true;
            else if (taskNum == "2" && task2 != "")
                isTask2Completed = true;
            else if (taskNum == "3" && task3 != "")
                isTask3Completed = true;
            else
            {
                Console.WriteLine("Invalid task number or task does not exist.");
                return;
            }

            Console.WriteLine($"Task {taskNum} marked as completed.");
        }

        static void ShowTasks()
        {
            Console.WriteLine("\n--- Task List ---");
            ShowTask(1, task1, isTask1Completed);
            ShowTask(2, task2, isTask2Completed);
            ShowTask(3, task3, isTask3Completed);
        }

        static void ShowTask(int number, string task, bool isCompleted)
        {
            if (task == "")
                Console.WriteLine($"Task {number}: [Empty]");
            else
                Console.WriteLine($"Task {number}: {task} - {(isCompleted ? "Completed" : "Pending")}");
        }

        static void RemoveTask()
        {
            Console.Write("Enter task number to remove (1-3): ");
            string taskNum = Console.ReadLine() ?? "";

            if (taskNum == "1" && task1 != "")
            {
                task1 = "";
                isTask1Completed = false;
                Console.WriteLine("Task 1 removed.");
            }
            else if (taskNum == "2" && task2 != "")
            {
                task2 = "";
                isTask2Completed = false;
                Console.WriteLine("Task 2 removed.");
            }
            else if (taskNum == "3" && task3 != "")
            {
                task3 = "";
                isTask3Completed = false;
                Console.WriteLine("Task 3 removed.");
            }
            else
            {
                Console.WriteLine("Invalid task number or task does not exist.");
            }
        }
    }
}
