using Microsoft.AspNetCore.Identity;

namespace Project4.DTO
{
    public class RoleDTO
    {
        public List<String> UsersRole { get; set; }
        public List<IdentityRole> ListRoles { get; set; }
    }
}
