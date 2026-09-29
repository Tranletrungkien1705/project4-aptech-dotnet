using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Project4.Repository;
using Project4.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Project4.Controllers
{
    public class ChapterController : Controller
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IPageRepository _pageRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ApplicationDbContext _context;
        public ChapterController(ApplicationDbContext context, IChapterRepository chapterRepository, IPageRepository pageRepository, ICommentRepository commentRepository)
        {
            _chapterRepository = chapterRepository;
            _pageRepository = pageRepository;
            _context = context;
            _commentRepository = commentRepository;
        }

        [AllowAnonymous]
        [HttpGet("GetChapterByName/{name}")]
        public async Task<ActionResult<Chapter>> GetChapterByName(string name)
        {
            var chapterDetail = await _chapterRepository.GetChapterByName(name);
            if (chapterDetail == null)
            {
                return NotFound();
            }
            return Ok(chapterDetail);
        }
        //------------------------------------List-Anh-------------------------------------------------------

        [AllowAnonymous]
        [HttpGet("/GetAllChapters")]
        public async Task<List<Chapter>> GetChapters()
        {
            List<Chapter> p = await _chapterRepository.GetAllAsync();
            List<Chapter> np = new List<Chapter>();
            String t = "";
            foreach (Chapter chapter in p)
            {
                Chapter ts = new Chapter();
                ts.Id = chapter.Id;
                //t = await _pageRepository.GetS3FilesImage(chapter.Images);
                ts.Name = chapter.Name;
                ts.SubName = chapter.SubName;
                ts.Content = chapter.Content;
                //ts.Images = t;
                ts.StoryId = chapter.StoryId;
                ts.IsPay = chapter.IsPay;
                np.Add(ts);
            }
            return np;
        }


        [AllowAnonymous]
        [HttpGet("GetDetailChaptersWithStoryId/{storyId}")]
        public async Task<ActionResult<ChapterStoryIdResponse>> GetDetailChaptersWithStoryId(string storyId)
        {
            var chapterDetail = await _chapterRepository.GetDetailChaptersWithStoryId(storyId);
            chapterDetail = await _chapterRepository.GetViewChapter(chapterDetail);
            if (chapterDetail == null)
            {
                return NotFound();
            }
            return Ok(chapterDetail);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpPost("AddChapterWithStoryId")]
        public async Task<AuthResult> AddChapterWithStoryId([FromForm] ChapterDTO dto)
        {
            string[] parts = dto.ChapterId.Split('+');
            if (parts.Length != 2)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                    "Invalid ID format: Missing '+' separator."
                },
                    Result = false
                };
            }

            string storyId = parts[0];
            string chapterId = dto.ChapterId;

            var chapterExists = await _chapterRepository.GetByIdAsync(chapterId);
            if (chapterExists != null)
            {
                return new AuthResult() { Errors = new List<string>() { "Chapter with ID already exists in the story" }, Result = false };
            }

            var chapter = new Chapter
            {
                Id = chapterId,
                Name = dto.ChapterName,
                SubName = dto.SubName,
                Content = dto.Content,
                StoryId = storyId,
                IsPay = false
            };
            await _chapterRepository.CreateAsync(chapter);

            // Thêm hình ảnh từ Pagee
            if (dto.Images != null)
            {
                int pageIndex = 1;
                foreach (var imageDto in dto.Images)
                {
                    if (imageDto != null)
                    {
                        string pageId = $"{chapterId}+page-{pageIndex++}";
                        var pageChapter = new Pagee
                        {
                            Id = pageId,
                            ChapterId = chapterId,
                            Images = await _pageRepository.WritingAnObjectAsync(imageDto.ImageLink)
                        };
                        await _pageRepository.CreateAsync(pageChapter);
                    }
                }
            }
            return new AuthResult() { Errors = new List<string>() { "Chapter, images, and comments added successfully." }, Result = true };
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpPut("UpdateChapter")]
        public async Task<AuthResult> UpdateChapter([FromForm] ChapterDTO dto)
        {
            if (string.IsNullOrEmpty(dto.ChapterId))
            {
                return new AuthResult()
                    {
                        Errors = new List<string>(){
                            "Invalid chapter ID."
                        },
                        Result = false
                    }; 
            }

            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == dto.ChapterId);
            if (chapter == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        $"Chapter with ID {dto.ChapterId} not found."
                    },
                    Result = false
                }; 
            }

            // Cập nhật thông tin chương
            chapter.Name = dto.ChapterName ?? chapter.Name;
            chapter.SubName = dto.SubName ?? chapter.SubName;
            chapter.Content = dto.Content ?? chapter.Content;
            chapter.IsPay = chapter.IsPay;

            // Xử lý hình ảnh
            if (dto.Images != null)
            {
                foreach (var imageDto in dto.Images)
                {
                    if (imageDto != null)
                    {
                        var filePath = await _pageRepository.WritingAnObjectAsync(imageDto.ImageLink);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            var page = await _context.Pagees.FirstOrDefaultAsync(p => p.Id == dto.PageId);
                            if (page == null)
                            {
                                page = new Pagee
                                {
                                    Id = page.Id,
                                    ChapterId = dto.ChapterId,
                                    Images = filePath
                                };
                                await _pageRepository.UpdateAsync(page);
                            }
                            else
                            {
                                page.Images = filePath;
                            }
                        }
                    }
                }
            }
            return new AuthResult() { Errors = new List<string>() { "Chapter updated successfully." }, Result = true };
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpDelete("DeleteChapter/{id}")]
        public async Task<AuthResult> DeleteChapter(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new AuthResult() { Errors = new List<string>() { "Invalid chapter ID." }, Result = false };
            }

            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return new AuthResult() { Errors = new List<string>() { $"Chapter with ID {id} not found." }, Result = false };
            }

            // Xóa tất cả hình ảnh liên quan đến chương
            var pages = await _context.Pagees.Where(p => p.ChapterId == id && p.IsDeleted == false).ToListAsync();
            foreach (var page in pages)
            {
                await _pageRepository.DeleteAsync(page.Id);
            }

            // Xóa tất cả bình luận liên quan đến chương
            var comments = await _context.Comments.Where(c => c.ChapterId == id && c.IsDeleted == false).ToListAsync();
            foreach (var item in comments)
            {
                await _commentRepository.DeleteAsync(item.Id);
            }

            // Xóa chương
            await _chapterRepository.DeleteAsync(id);

            return new AuthResult() { Errors = new List<string>() { $"Chapter with ID {id} and all related data have been deleted." }, Result = true };
        }
    }
}
