namespace CollegeBackend.Role;

public static class RoleFlags
{
    public const int User = 1 << 0,
        Moderator = 1 << 1,
        Administrator = 1 << 2,
        DatabaseAdministrator = 1 << 3,
        SystemAdministrator = 1 << 4;
}