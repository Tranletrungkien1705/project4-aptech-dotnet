
using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Project4.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project4.Repository;
using static Amazon.S3.Util.S3EventNotification;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Project4.Repository
{
    public interface IChapterRepository : IBaseRepository<Chapter> 
    {
        //Task<List<StoryChapterDTO>> GetDetailStories();
        Task<List<ChapterResponse>> GetDetailPagesWithChapterId(string chapterId);
        Task<List<ChapterResponse>> GetLinkPagesWithChapterId(List<ChapterResponse> chapterDetail);
        Task<List<Chapter>> GetChapterByName(string name);
        Task<List<ChapterStoryIdResponse>> GetDetailChaptersWithStoryId(string storyId);
        Task<List<ChapterStoryIdResponse>> GetViewChapter(List<ChapterStoryIdResponse> chapterDetail);
        //Task<List<ChapterStoryIdResponse>> GetLinkPicturesChaptersWithStoryId(List<ChapterStoryIdResponse> chapterDetail);
    }

    public class ChapterRepository : BaseRepository<Chapter>, IChapterRepository
    {
        public IPageRepository _pageRepository;
        public IViewdRepository _viewdRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        protected readonly UserManager<CustomUser> _userManager;
        public ChapterRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor, IPageRepository pageRepository, IViewdRepository viewdRepository) : base(context, userManager, contextAccessor)
        {
            _pageRepository = pageRepository;
            _viewdRepository = viewdRepository;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        
        
        public async Task<List<ChapterResponse>> GetDetailPagesWithChapterId(string chapterId)
        {
            var httpContext = _contextAccessor.HttpContext;
            var userId = httpContext.Items["UserId"]?.ToString();
            var query = from c in _context.Chapters
                        where c.Id == chapterId
                        select new ChapterResponse
                        {
                            StoryIdChapter = c.StoryId,
                            ChapterId = c.Id,
                            ChapterName = c.SubName,
                            ChapterContent = c.Content,
                            Pages = (from p in _context.Pagees
                                     where p.ChapterId == c.Id
                                     select new PageeResponse
                                     {
                                         PageId = p.Id,
                                         Content = p.Content,
                                         Images = p.Images 
                                     }).ToList<PageeResponse>()
                        };
            var view = new Viewed();
            view.ChapterId = chapterId;
            view.UserId = userId;
            await _viewdRepository.CreateAsync(view);
            return await query.ToListAsync();
        }
        public async Task<List<Chapter>> GetChapterByName(string name)
        {
            var query = from c in _context.Chapters.AsQueryable()
                        where c.Name.ToLower().Contains(name.ToLower())

                        select c;

            return await query.ToListAsync();
        }
        public async Task<List<ChapterResponse>> GetLinkPagesWithChapterId(List<ChapterResponse> chapterDetail)
        {
            foreach (var page in chapterDetail)
            {
                foreach (var item in page.Pages)
                {
                    item.Images = await _pageRepository.GetS3FilesImage(item.Images);
                }
            }
            return chapterDetail;
        }

        public async Task<List<ChapterStoryIdResponse>> GetDetailChaptersWithStoryId(string storyId)
        {
            var query = from s in _context.Stories
                        where s.Id == storyId
                        select new ChapterStoryIdResponse
                        {
                            StoryId = s.Id,
                            StoryName = s.Name,
                            Chapters = (from c in _context.Chapters
                                     where c.StoryId == s.Id
                                     select new ChapterDetailDTO
                                     {
                                         Id = c.Id,
                                         Name = c.Name,
                                         SubName = c.SubName,
                                         Content = c.Content,
                                         CreatedTime = c.CreatedTime,
                                         IsPay = c.IsPay

                                     }).ToList<ChapterDetailDTO>()
                        };

            return await query.ToListAsync();
        }

        public async Task<List<ChapterStoryIdResponse>> GetViewChapter(List<ChapterStoryIdResponse> chapterDetail)
        {
            foreach(var cde in chapterDetail)
            {
                foreach(var v in cde.Chapters)
                {
                    v.View = _viewdRepository.GetChapterViewCounts(v.Id);
                }
            }
            return chapterDetail;
        }


    }
}
