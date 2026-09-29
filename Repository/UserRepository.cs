using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project4.Data;
using Project4.DTO;
using Project4.Models;

namespace Project4.Repository
{
    public interface IUserRepository
    {
        Task<UserDTO> GetDetailStoriesWithUserId(string userId);
        Task<List<UserDTO>> GetDetailStoriesWithAllUser();
    }

    public class UserRepository : IUserRepository
    {
        protected readonly ApplicationDbContext _context;
        private readonly IStoryRepository _storyRepository;
        public UserRepository(IStoryRepository storyRepository, ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor, IViewdRepository viewdRepository) 
        {
            _context= context;
            _storyRepository= storyRepository;
        }
        public async Task<UserDTO> GetDetailStoriesWithUserId(string userId)
        {
            var query = from u in _context.CustomUsers
                        where u.Id == userId
                        select new UserDTO
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Gender = u.Gender,
                            DateOfBirth = u.DateOfBirth,
                            Position = u.Position,
                            Stories = (from su in _context.Users
                                       join s in _context.Stories on su.Id equals s.CustomUserId
                                       where su.Id == u.Id
                                       select new StoryUserDTO
                                       {
                                           StoryId = su.Id,
                                           Name = s.Name,
                                           SubName = s.SubName,
                                           Description = s.Description,
                                           Image = s.Image,
                                           Status = s.Status
                                       }).ToList()
                        };
            var t = await query.ToListAsync();

            return t.First();
        }
        public async Task<List<UserDTO>> GetDetailStoriesWithAllUser()
        {
            var query = from u in _context.CustomUsers
                        select new UserDTO
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Gender = u.Gender,
                            DateOfBirth = u.DateOfBirth,
                            Position = u.Position,
                            Stories = (from su in _context.Users
                                       join s in _context.Stories on su.Id equals s.CustomUserId
                                       where su.Id == u.Id
                                       select new StoryUserDTO
                                       {
                                           StoryId = su.Id,
                                           Name = s.Name,
                                           SubName = s.SubName,
                                           Description = s.Description,
                                           Image = s.Image,
                                           Status = s.Status
                                       }).ToList()
                        };

            return await query.ToListAsync();
        }
    }
}
