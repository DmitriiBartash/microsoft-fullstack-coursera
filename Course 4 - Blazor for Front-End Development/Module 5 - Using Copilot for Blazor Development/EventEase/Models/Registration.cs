namespace EventEase.Models;

public sealed class Registration
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
