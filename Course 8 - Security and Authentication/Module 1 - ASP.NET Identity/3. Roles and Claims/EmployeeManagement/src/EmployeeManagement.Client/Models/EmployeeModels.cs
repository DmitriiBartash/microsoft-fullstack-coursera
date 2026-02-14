namespace EmployeeManagement.Client.Models;

public record EmployeeResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Position,
    string Department);
