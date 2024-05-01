using Project4.Models;
using Microsoft.AspNetCore.Identity;

namespace Project4.Models
{
    public class CustomUser: IdentityUser
    {
        public CustomUser(string userName) : base(userName)
        {
        }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Position { get; set; }
        public bool? IsLocked { get; set; }

    }
}
