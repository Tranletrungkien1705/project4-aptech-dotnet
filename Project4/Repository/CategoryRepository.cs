
using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Project4.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Project4.Repository
{
    public interface ICategoryRepository : IBaseRepository<Category> 
    {
        Task<List<CategoryDTO>> GetDetailStoriesWithCategoryId(string categoryId);
        Task<List<Category>> GetCategoriesByName(string name);
        Task<List<CategoryDTO>> GetLinkPagesWithChapterId(List<CategoryDTO> storyDetail);
    }

    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {

        public IPageRepository _pageRepository;
        private string _imageS3;

        public CategoryRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor, IPageRepository pageRepository) : base(context, userManager, contextAccessor)
        {
            _pageRepository = pageRepository;
        }

        public async Task<List<CategoryDTO>> GetDetailStoriesWithCategoryId(string categoryId)
        {

            var query = from c in _context.Categories
                        where c.Id == categoryId
                        select new CategoryDTO
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Stories = (from sc in _context.Story_Categories
                                       join s in _context.Stories on sc.StoryId equals s.Id
                                       where sc.CategoryId == c.Id
                                       && s.IsDeleted == false && sc.IsDeleted == false
                                       select new StoryResponse
                                       {
                                           Id = s.Id,
                                           Name = s.Name,
                                           SubName = s.SubName,
                                           Description = s.Description,
                                           Image = s.Image,
                                           Status = s.Status

                                       }).ToList()
                        };

            return await query.ToListAsync();
        }
        public async Task<List<CategoryDTO>> GetLinkPagesWithChapterId(List<CategoryDTO> categoryDTOs)
        {
            foreach (var page in categoryDTOs)
            {
                foreach (var item in page.Stories)
                {
                    item.Image = await _pageRepository.GetS3FilesImage(item.Image);
                }
            }
            return categoryDTOs;
        }

        public async Task<List<Category>> GetCategoriesByName(string name)
        {
            var query = from c in _context.Categories.AsQueryable()
                        where c.Name.ToLower().Contains(name.ToLower())

                        select c;

            return await query.ToListAsync();
        }

        /*   public async Task<List<ChapterDTO>> GetLinkPagesWithChapterId(List<ChapterDTO> chapterDetail)
           {
               foreach (var page in chapterDetail)
               {
                   foreach (var item in page.Pages)
                   {
                       item.Images = await _pageRepository.GetS3FilesImage(item.Images);
                   }
               }
               return chapterDetail;
           }*/
    }
}
