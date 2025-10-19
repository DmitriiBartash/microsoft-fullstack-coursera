namespace MinimalApiDemo.Models
{
    public class TaskItem(int id, string name, bool isCompleted)
    {
        public int Id { get; set; } = id;
        public string Name { get; set; } = name;
        public bool IsCompleted { get; set; } = isCompleted;
    }
}
