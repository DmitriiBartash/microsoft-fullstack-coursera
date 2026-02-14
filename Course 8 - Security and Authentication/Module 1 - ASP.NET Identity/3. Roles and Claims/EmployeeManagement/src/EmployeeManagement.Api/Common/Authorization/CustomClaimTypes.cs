namespace EmployeeManagement.Api.Common.Authorization;

public static class CustomClaimTypes
{
    public const string Department = "Department";
    public const string AccessLevel = "AccessLevel";
}

public static class Departments
{
    public const string IT = "IT";
    public const string HR = "HR";
    public const string Finance = "Finance";
}

public static class AccessLevels
{
    public const string Read = "Read";
    public const string Write = "Write";
    public const string Full = "Full";
}
