
using Project4.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Project4.Repository;

namespace Project4.Repository
{
    public interface IStoryRepository : IBaseRepository<Story>
    {
        Task<List<Story>> GetStoriesByName(string name);
        Task<List<UserDTO>> GetLinkPictuersStoriesWithUserId(List<UserDTO> storyDetail);
        Task<List<StoryResponse>> GetAllStoriesView(List<StoryResponse> allStories);
        Task<List<StoryResponse>> GetAllStoies();
        Task<StoryResponse> GetStoiesById(string id);
        Task<StoryResponse> GetOneStoriesView(StoryResponse stories);
        Task<UserDTO> GetAllStoriesViewByOneUserDTO(UserDTO user);
        Task<List<UserDTO>> GetAllStoriesViewByUserDTO(List<UserDTO> users);
        List<StoryCategory> GetCategoryNameByStoryId(string storyId);
    }

    public class StoryRepository : BaseRepository<Story>, IStoryRepository
    {
        private readonly IPageRepository _pageRepository;
        private readonly IViewdRepository _viewdRepository;
        private readonly IBaseRepository<Story> _baseRepository;
        public StoryRepository(IPageRepository pageRepository, ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor, IViewdRepository viewdRepository, IBaseRepository<Story> baseRepository) : base(context, userManager, contextAccessor)
        {
            _pageRepository = pageRepository;
            _viewdRepository = viewdRepository;
            _baseRepository = baseRepository;
        }



        public async Task<List<StoryResponse>> GetAllStoies()
        {
            var allStory = await _baseRepository.GetAllAsync();
            var allStoryDTO = new List<StoryResponse>();
            foreach (var story in allStory)
            {
                var storyDTO = new StoryResponse();
                storyDTO.Id = story.Id;
                storyDTO.Name = story.Name;
                storyDTO.Description = story.Description;
                storyDTO.Image = story.Image;
                storyDTO.CreatedTime = story.CreatedTime;
                storyDTO.UpdatedTime = story.UpdatedTime;
                var author = new Author();
                if (story.CustomUserId != null)
                {
                    var t = await _context.Users.Where(x => x.Id == story.CustomUserId).FirstOrDefaultAsync();
                    author.UserId = story.CustomUserId;
                    author.UserName = t.UserName;
                    storyDTO.Author= author;
                }
                else
                {
                    author.UserId = "N/A"; 
                    author.UserName = "N/A";
                    storyDTO.Author = author;
                }
                storyDTO.Status = story.Status;
                var cate = GetCategoryNameByStoryId(story.Id);
                storyDTO.StoryCategory = cate;
                allStoryDTO.Add(storyDTO);
            }
            return allStoryDTO;
        }


        public List<StoryCategory> GetCategoryNameByStoryId(string storyId)
        {
            var categoryInfo = _context.Story_Categories
                .Where(sc => sc.StoryId == storyId && sc.IsDeleted == false)
                .Select(sc => new StoryCategory
                {
                    CateId = sc.CategoryId,
                    CateName = sc.Categories.Name
                })
                .ToList();

            return categoryInfo;
        }

        public async Task<StoryResponse> GetStoiesById(String id)
        {
            var story = await _baseRepository.GetByIdAsync(id);
                var storyDTO = new StoryResponse();
                storyDTO.Id = story.Id;
                storyDTO.Name = story.Name;
                storyDTO.SubName = story.SubName;
                storyDTO.Description = story.Description;
                storyDTO.Image = story.Image;
                storyDTO.CreatedTime = story.CreatedTime;
                storyDTO.UpdatedTime = story.UpdatedTime;
                var author = new Author();
                if (story.CustomUserId != null)
                {
                    var t = await _context.Users.Where(x => x.Id == story.CustomUserId).FirstOrDefaultAsync();
                    author.UserId = story.CustomUserId;
                    author.UserName = t.UserName;
                    storyDTO.Author = author;
                }
                else
                {
                    author.UserId = "N/A";
                    author.UserName = "N/A";
                    storyDTO.Author = author;
                }
                var cate = GetCategoryNameByStoryId(story.Id);
                storyDTO.Status = story.Status;
                storyDTO.StoryCategory = cate;
            
            return storyDTO;
        }


        public async Task<List<StoryResponse>>GetAllStoriesView(List<StoryResponse> allStories)
        {
            foreach (var story in allStories)
            {
               story.View = _viewdRepository.GetStoryViewCounts2(story.Id);
                story.Image = await _pageRepository.GetS3FilesImage(story.Image);
            }
            return allStories;
        }
        public async Task<StoryResponse> GetOneStoriesView(StoryResponse stories)
        {
            stories.View = _viewdRepository.GetStoryViewCounts2(stories.Id);
            stories.Image = await _pageRepository.GetS3FilesImage(stories.Image);
           
            return stories;
        }
        public async Task<List<UserDTO>> GetAllStoriesViewByUserDTO(List<UserDTO> users)
        {
            foreach (var user in users)
            {
                foreach (var story in user.Stories)
                {
                    story.View = _viewdRepository.GetStoryViewCounts2(story.StoryId);
                    story.Image = await _pageRepository.GetS3FilesImage(story.Image);
                }
            }
            return users;
        }
        public async Task<UserDTO> GetAllStoriesViewByOneUserDTO(UserDTO user)
        {
                foreach (var story in user.Stories)
                {
                    story.View = _viewdRepository.GetStoryViewCounts2(story.StoryId);
                    story.Image = await _pageRepository.GetS3FilesImage(story.Image);
                }
            return user;
        }

        public async Task<List<UserDTO>> GetLinkPictuersStoriesWithUserId(List<UserDTO> storyDetail)
        {
            foreach (var story in storyDetail)
            {
                foreach (var item in story.Stories)
                {
                    item.Image = await _pageRepository.GetS3FilesImage(item.Image);
                }
            }
            return storyDetail;
        }


        public async Task<List<Story>> GetStoriesByName(string name)
        {
            var query = from s in _context.Stories.AsQueryable()
                        where s.Name.ToLower().Contains(name.ToLower())

                        select s;

            return await query.ToListAsync();
        }

    }
}
