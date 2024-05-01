using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Project4.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using NuGet.Protocol.Core.Types;
using System.Drawing.Text;
using static Amazon.S3.Util.S3EventNotification;

using Project4.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Project4.Controllers
{
    [Route("api")]
    [ApiController]
    public class StoryController : APIBaseController<Story>
    {
        private readonly IStoryRepository _storyRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IStoryCateRepository _storyCateRepository;
        private readonly ApplicationDbContext _context;
        protected readonly UserManager<CustomUser> _userManager;
        protected readonly string currentUserId = "";
        protected readonly IHttpContextAccessor _contextAccessor;
        public StoryController(ICategoryRepository categoryRepository, IStoryRepository storyRepository, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor, ApplicationDbContext context, IPageRepository pageRepository, IUserRepository userRepository, IStoryCateRepository storyCateRepository) : base(storyRepository,userManager)
        {
             _storyRepository = storyRepository;
            _userRepository = userRepository;
            _context = context;
            _pageRepository = pageRepository;
            _categoryRepository = categoryRepository;
            _storyCateRepository= storyCateRepository;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _categoryRepository = categoryRepository;
            var currentUser = _userManager.GetUserAsync(_contextAccessor.HttpContext.User).GetAwaiter().GetResult();
            if (currentUser != null)
            {
                this.currentUserId = currentUser.Id;
            }
        }


        //-------------------------------------Add-Anh----------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpPost("/AddStory")]

        public async Task<AuthResult> AddStory([FromBody] StoryDTO storyDTO)
        {
            int t = 0;
            if (storyDTO == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                    "khong duoc de trong"
                },
                    Result = false
                };
            }
            Story story = new Story();
            story.Id = storyDTO.Id;
            story.Image = await _pageRepository.WritingAnObjectAsync(storyDTO.Image) == "" ? "" : await _pageRepository.WritingAnObjectAsync(storyDTO.Image);
            story.Name = storyDTO.Name;
            story.SubName = storyDTO.SubName;
            story.Description = storyDTO.Description;
            story.Status = storyDTO.Status;
            story.UpdatedUser = currentUserId;
            story.UpdatedTime = DateTime.Now;
            story.IsDeleted = false;
            await _storyRepository.CreateAsync(story);
            foreach (var cate in storyDTO.cateDTOs) 
            {
                Story_Category sc = new Story_Category();
                sc.Id = Guid.NewGuid().ToString();
                sc.StoryId= story.Id;
                sc.CategoryId = cate.cateId;
                _storyCateRepository.CreateAsync(sc);
            }
            return new AuthResult()
                {
                    Errors = new List<string>(){
                        "you add the story successfully"
                    },
                    Result = true
                };
        }

        //----------------------------------------Update-Anh-------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpPut("/UpdateAnhStory/{id}")]
        public async Task<ActionResult<AuthResult>> UpdateStory([FromBody] StoryDTO storyDTO)
        {
            if (storyDTO == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                    "NO EMTY"
                },
                    Result = false
                };
            }

            var existingStory = await _context.Stories.FindAsync(storyDTO.Id);
            if (existingStory == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        $"Error: No existing story found with ID {storyDTO}."
                    },
                    Result = false
                }; 
            }
            string imageResult = await _pageRepository.WritingAnObjectAsync(storyDTO.Image) == "" ? "" : await _pageRepository.WritingAnObjectAsync(storyDTO.Image);
            existingStory.Image = string.IsNullOrEmpty(imageResult) ? "" : imageResult;
            existingStory.Name = storyDTO.Name;
            existingStory.SubName = storyDTO.SubName;
            existingStory.Description = storyDTO.Description;
            existingStory.Status = storyDTO.Status;
            existingStory.UpdatedUser = currentUserId;
            existingStory.UpdatedTime = DateTime.Now;
            existingStory.IsDeleted = false;
            await _storyRepository.UpdateAsync(existingStory);

            foreach (var cate in _storyRepository.GetCategoryNameByStoryId(existingStory.Id))
            {
                var listcate =_context.Story_Categories.Where(x => x.StoryId == storyDTO.Id && x.CategoryId == cate.CateId && x.IsDeleted == false).ToList() ;
                foreach(var ca in listcate)
                {
                    await _storyCateRepository.DeleteAsync(ca.Id);
                }
            }
            foreach (var cate in storyDTO.cateDTOs)
            {
                Story_Category sc = new Story_Category();
                sc.Id = Guid.NewGuid().ToString();
                sc.StoryId = existingStory.Id;
                sc.CategoryId = cate.cateId;
                _storyCateRepository.CreateAsync(sc);
            }

            return new AuthResult()
            {
                Errors = new List<string>(){
                        "you update the story successfully"
                    },
                Result = true
            };
        }


        //------------------------------------List-Anh-------------------------------------------------------

        [AllowAnonymous]
        [HttpGet("/GetAllStories")]
        public async Task<List<StoryResponse>> GetStories()
        {
            List<StoryResponse> p = await _storyRepository.GetAllStoies();
            p = await _storyRepository.GetAllStoriesView(p);
            return p;

        }
        [AllowAnonymous]
        [HttpGet("/GetStoryById")]
        public async Task<StoryResponse> GetStoriesById(String id)
        {
            StoryResponse p = await _storyRepository.GetStoiesById(id);
            p = await _storyRepository.GetOneStoriesView(p);
            return p;

        }
        //------------------------------------------------------------Xoa-Anh------------------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpDelete("/DeleteAnhStory/{id}")]
        public async Task<ActionResult<AuthResult>> DeleteStory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        "Invalid ID."
                },
                    Result = false
                }; 
            }

            var Story = await _context.Stories.FindAsync(id);
            if (Story == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        $"No page found with ID {id}."
                },
                    Result = false
                }; 
            }

            foreach (var cate in _storyRepository.GetCategoryNameByStoryId(Story.Id))
            {
                var listcate = _context.Story_Categories.Where(x => x.StoryId == Story.Id && x.IsDeleted == false).ToList();
                foreach (var stca in listcate)
                {
                    await _storyCateRepository.DeleteAsync(stca.Id);
                }
            }

            return new AuthResult() { Errors = new List<string>() { "Page deleted successfully." }, Result = true };
        }



        [AllowAnonymous]
        [HttpGet("/GetStoriesByName/{name}")]
        public async Task<ActionResult<Story>> GetStoriesByName(string name)
        {
            var storyDetail = await _storyRepository.GetStoriesByName(name);
            if (storyDetail == null)
            {
                return NotFound();
            }
            return Ok(storyDetail);
        }

        [AllowAnonymous]
        [HttpGet("GetDetailStoriesWithCategoryId/{categoryId}")]
        public async Task<ActionResult<CategoryDTO>> GetDetailStoriesWithCategoryId(string categoryId)
        {
            var storyDetail = await _categoryRepository.GetDetailStoriesWithCategoryId(categoryId);
            var imageStories = await _categoryRepository.GetLinkPagesWithChapterId(storyDetail);
            if (imageStories == null)
            {
                return NotFound();
            }
            return Ok(imageStories);
        }

    }
}
