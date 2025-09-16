namespace DeserializationLab.Models;

public class Person
{
    public required string UserName { get; set; }
    public int UserAge { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
}
