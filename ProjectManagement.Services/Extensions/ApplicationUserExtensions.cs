using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Extensions;

public static class ApplicationUserExtensions
{
    public static string FullName(this ApplicationUser user)
    {
        return $"{user?.FirstName} {user?.LastName}".Trim();
    }
}
