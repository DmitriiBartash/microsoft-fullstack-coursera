namespace EmployeeManagement.Api.Features.Users.Contracts;

public record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles,
    Dictionary<string, string> Claims);
