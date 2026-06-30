namespace SecureDataCore.Models;

public sealed class User(string username, string role)
{
    public string Username { get; } = username;
    public string Role { get; } = role;
}

// Named role constants keep authorization checks free of magic strings.
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
