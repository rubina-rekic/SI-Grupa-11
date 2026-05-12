namespace PostRoute.Domain.Entities;

public static class UserRole
{
    public const string Administrator = "Administrator";
    public const string Dispatcher = "Dispatcher";
    public const string PostalWorker = "PostalWorker";
    
    public static readonly string[] AllRoles = [Administrator, Dispatcher, PostalWorker];
    
    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
}
