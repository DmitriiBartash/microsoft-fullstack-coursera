namespace EmployeeManagement.Api.Common.Authorization;

public static class PolicyNames
{
    public const string AdminOnly = "AdminOnly";
    public const string ManagementAccess = "ManagementAccess";
    public const string RequireWriteAccess = "RequireWriteAccess";
    public const string RequireFullAccess = "RequireFullAccess";
}
