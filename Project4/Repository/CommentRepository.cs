
using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project4.Repository;

namespace Project4.Repository
{
    public interface ICommentRepository : IBaseRepository<Comment> 
    {
        Task<List<UserCommentDTO>> GetDetailCommentsWithUser(string userId);
        Task<List<Comment>> GetCommentsByName(string content);
    }

    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {

        }
        public async Task<List<UserCommentDTO>> GetDetailCommentsWithUser(string userId)
        {
            var query = from u in _context.Users
                        where u.Id == userId
                        select new UserCommentDTO
                        {
                            UserId = u.Id,
                            Comments = (from ch in _context.Chapters
                                        join c in _context.Comments on ch.Id equals c.ChapterId
                                        join v in _context.Vieweds on ch.Id equals v.ChapterId
                                        where v.UserId == u.Id
                                       select new CommentUserDTO
                                       {
                                           Content = c.Content,
                                           Feeling = c.Feeling
                                       }).ToList()
                        };

            return await query.ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByName(string content)
        {
            var query = from c in _context.Comments.AsQueryable()
                        where c.Content.ToLower().Contains(content.ToLower())

                        select c;

            return await query.ToListAsync();
        }

    }
}
