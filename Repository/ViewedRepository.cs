
using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
using Project4.Repository;

namespace Project4.Repository
{
    public interface IViewdRepository : IBaseRepository<Viewed> 
    {
        string GetChapterViewCounts(string id);
        IActionResult GetStoryViewCounts();
        string GetStoryViewCounts2(string id);
    }

    public class ViewdRepository : BaseRepository<Viewed>, IViewdRepository
    {
        public ViewdRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {

        }

        public string GetChapterViewCounts(string id)
        {
            int viewCount = _context.Vieweds
                .Where(x => x.ChapterId == id)
                .Count();  // Directly count the number of views for the given ChapterId

            if (viewCount == 0)
            {
                return "N/A";
            }

            return viewCount.ToString();  // Return the view count as a string
        }



        public IActionResult GetStoryViewCounts()
        {
            var storyViewCounts = _context.Chapters
                .Where(c => c.StoryId != null)
                .Select(c => new { c.StoryId, ChapterId = c.Id })
                .ToList()
                .GroupBy(c => c.StoryId)
                .Select(g => new {
                    StoryId = g.Key,
                    ViewCount = g.Sum(d => _context.Vieweds.Count(v => v.ChapterId == d.ChapterId))
                }).ToList();

            //return Ok(storyViewCounts);
            return null;
        }

        public string GetStoryViewCounts2(string id) {
            var storyViewCounts = _context.Chapters
                .Where(c => c.StoryId == id)
                .Select(c => new { c.StoryId, ChapterId = c.Id })
                .ToList()
                .GroupBy(c => c.StoryId)
                .Select(g => g.Sum(d => _context.Vieweds.Count(v => v.ChapterId == d.ChapterId))).FirstOrDefault();

            if (storyViewCounts == null)
            {
                return "N/A";
            }

            return storyViewCounts.ToString();
        }
    }
}
