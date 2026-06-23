using SecureApiCore.Models;

namespace SecureApiCore;

/// <summary>
/// In-memory demo accounts shared by the API, the tests and the Studio. A real store would persist
/// users and compare a salted password hash, never plaintext.
/// </summary>
public class UserStore
{
    private static readonly List<User> Users =
    [
        new User { Username = "admin", Password = "password", Roles = ["Admin", "User"] },
        new User { Username = "alice", Password = "pa55",     Roles = ["User"] },
    ];

    public IReadOnlyList<User> All => Users;

    /// <summary>Returns the matching user, or <c>null</c> when the credentials are invalid.</summary>
    public User? Find(string username, string password) =>
        Users.FirstOrDefault(u => u.Username == username && u.Password == password);
}
