using Project4.Data;
using Project4.Models;
using Microsoft.AspNetCore.Identity;

namespace Project4.Repository
{
    public interface IUserRefreshTokensRepository : IBaseRepository<UserRefreshTokens>
    {
    }

    public class UserRefreshTokensRepository : BaseRepository<UserRefreshTokens>, IUserRefreshTokensRepository
    {
        public UserRefreshTokensRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {

        }
    }
}
