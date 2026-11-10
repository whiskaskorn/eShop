using AppDbContext;
using models;

namespace RoleService;

public class Role
{
    public async Task UserRole(UserContext db, User user)
    {
        if(user.Roles == "Admin")
        {
            
        }
    }
}