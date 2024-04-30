using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using Project4.Models;
using Project4.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Amazon;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project4.DTO;
using Microsoft.EntityFrameworkCore.Migrations;
using Project4.Data;
using Project4.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;


namespace Project4.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageRepository _pageRepository;
        private readonly ApplicationDbContext _context;
        private readonly IChapterRepository _chapterRepository;

        public PagesController(IChapterRepository chapterRepository, ApplicationDbContext context, IBaseRepository<Pagee> repository, IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
            _context = context;
            _chapterRepository = chapterRepository;
        }
        //-------------------------------------Add-Anh----------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpPost("/UpAnh")]
        
        public async Task<AuthResult> UpS3([FromBody] PageeDTO pageDTO)
        {
            int t = 0;
            if (pageDTO == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        "Error"
                    },
                    Result = false
                };
            }
            foreach (var image in pageDTO.Images)
            {
                t++;
                Pagee page = new Pagee();
                page.Id = pageDTO.ChapterId +"-" + t;
                page.Images = await _pageRepository.WritingAnObjectAsync(image) == "" ? "" : await _pageRepository.WritingAnObjectAsync(image);
                page.Content = pageDTO.Content;
                page.ChapterId = pageDTO.ChapterId;
                await _pageRepository.CreateAsync(page);
            }
            return new AuthResult()
            {
                Errors = new List<string>(){
                    "Add Page Successfully"
                },
                Result = true
            };
        }

        //----------------------------------------Update-Anh-------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpPut("/UpdateAnh")]
        public async Task<AuthResult> UpdatePagee([FromBody] PageeDTO pageDTO)
        {
            if (pageDTO == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        "Error: Input data is invalid."
                },
                    Result = false
                }; 
            }

            foreach (var image in pageDTO.Images)
            {
                var existingPage = await _context.Pagees.FindAsync(pageDTO.PageId);
                if (existingPage == null)
                {
                    // Handle case where the record does not exist
                    return new AuthResult()
                    {
                        Errors = new List<string>(){
                            $"Error: No existing page found with ID {pageDTO.PageId}."
                    },
                        Result = false
                    }; 
                }

                if (!string.IsNullOrEmpty(existingPage.Images))
                {
                    await _pageRepository.DeleteAsync(existingPage.Images);
                }
                var t = 1;
                string imageResult = await _pageRepository.WritingAnObjectAsync(image);
                var newPage = new Pagee();
                newPage.Id = pageDTO.PageId + "-" + t;
                newPage.Images = string.IsNullOrEmpty(imageResult) ? "" : imageResult;
                newPage.Content = pageDTO.Content;
                newPage.ChapterId = pageDTO.ChapterId;
                _pageRepository.CreateAsync(newPage);
            }

            return new AuthResult()
            {
                Errors = new List<string>(){
                        "ok roi"
                },
                Result = true
            }; 
        }

        //------------------------------------List-Anh-------------------------------------------------------

        [AllowAnonymous]
        [HttpGet("/GetAllPagee")]
        public async Task<List<Pagee>> GetPages()
        {
            List<Pagee> p = await _pageRepository.GetAllAsync();
            List<Pagee> np = new List<Pagee>();
            String t = "";
            foreach (Pagee page in p)
            {
                Pagee ts = new Pagee();
                t = await _pageRepository.GetS3FilesImage(page.Images);
                ts.ChapterId = page.ChapterId;
                ts.Images = t;
                ts.Id = page.Id;
                ts.Content = page.Content;
                np.Add(ts);
            }
            return np;
        }

        //-----------------------------------------Xoa-Anh----------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER,AUTHOR")]
        [HttpDelete("/DeleteAnh/{id}")]
        public async Task<AuthResult> DeletePagee(string id)
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

            var page = await _context.Pagees.FindAsync(id);
            if (page == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                        $"No page found with ID {id}."
                    },
                    Result = false
                }; 
            }

            if (!string.IsNullOrEmpty(page.Images))
            {
                await _pageRepository.DeleteAsync(page.Images);
            }
            

            return new AuthResult() { Errors = new List<string>() { "Page deleted successfully" }, Result = true };
        }


        //------------------------------------------Tim-Kim-Anh-Theo-Content-----------------------------------
        [AllowAnonymous]
        [HttpGet("GetPagesByContent/{content}")]
        public async Task<ActionResult<Pagee>> GetPagesByContent(string content)
        {
            var pageDetail = await _pageRepository.GetPagesByContent(content);
            if (pageDetail == null)
            {
                return NotFound();
            }
            return Ok(pageDetail);
        }

        [AllowAnonymous]
        [HttpGet("GetDetailPagesWithChapterId/{chapterId}")]
        public async Task<ActionResult<ChapterResponse>> GetDetailPagesWithChapterId(string chapterId)
        {
            var pageDetail = await _chapterRepository.GetDetailPagesWithChapterId(chapterId);
            var pageDetails = await _chapterRepository.GetLinkPagesWithChapterId(pageDetail);
            if (pageDetails == null)
            {
                return NotFound();
            }
            return Ok(pageDetails);
        }


    }
}