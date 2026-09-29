using Microsoft.AspNetCore.Identity;
using Project4.Data;
using Project4.DTO;
using Project4.Models;

namespace Project4.Repository
{
    public interface IStoryCateRepository : IBaseRepository<Story_Category>
    {
    }

    public class StoryCateRepository : BaseRepository<Story_Category>, IStoryCateRepository
    {
        public StoryCateRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {
        }
    }
}
