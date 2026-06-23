namespace SecureApiCore.Models;

public class User
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public IReadOnlyList<string> Roles { get; set; } = [];
}
