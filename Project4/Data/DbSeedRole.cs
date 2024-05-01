using Microsoft.AspNetCore.Identity;

namespace Project4.Data
{
    public class DbSeedRole
    {
        private readonly RoleManager<IdentityRole> _manager;
        public DbSeedRole(RoleManager<IdentityRole> manager)
        {
            _manager = manager;
        }
        public async Task RoleData()
        {
            await _manager.CreateAsync(new IdentityRole { Name = "AUTHOR", NormalizedName = "AUTHOR" });
            //await _manager.CreateAsync(new IdentityRole { Name = "MANAGER", NormalizedName = "MANAGER" });
            //await _manager.CreateAsync(new IdentityRole { Name = "USER", NormalizedName = "USER" });
        }
    }
}
