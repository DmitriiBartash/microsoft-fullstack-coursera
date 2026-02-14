namespace EmployeeManagement.Client.Models;

public record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles,
    Dictionary<string, string> Claims);

public record AssignRoleRequest(string Role);

public record AddClaimRequest(string ClaimType, string ClaimValue);
