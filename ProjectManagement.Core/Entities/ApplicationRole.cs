using Microsoft.AspNetCore.Identity;
namespace ProjectManagement.Core.Entities;
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
