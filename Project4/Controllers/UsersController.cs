using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project4.Repository;
using Microsoft.EntityFrameworkCore.Migrations;
using Project4.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Project4.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : Controller
    {
        protected readonly UserManager<CustomUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly IJWTManagerRepository _jWTManagerRepository;
        private readonly IStoryRepository _storyRepository;
        private readonly IUserRepository _userRepository;
        public UsersController(IHttpContextAccessor contextAccessor, UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager,IJWTManagerRepository jWTManagerRepository, IStoryRepository storyRepository, IUserRepository userRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _jWTManagerRepository = jWTManagerRepository;
            _storyRepository = storyRepository;
            _userRepository = userRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Authorizations()
        {
            var entity = await _jWTManagerRepository.GetAllCustomUser();
            return Ok(entity);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet("[controller]/[action]/{id}")]
        public async Task<ActionResult> UpdateUser(string id)
        {   
            var x = await _userManager.FindByIdAsync(id);
            return Ok(x);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<ActionResult> DoUpdateUser([Bind("Id,UserName,Email,PhoneNumber")] CustomUser customUser)
        {
            if (customUser != null)
            {
                string userId = String.Empty;
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    userId = identity.FindFirst(ClaimTypes.Sid).Value;
                }
                var x = await _userManager.FindByIdAsync(userId);
                if (x != null)
                {
                    x.PhoneNumber = customUser.PhoneNumber;
                    x.UserName = customUser.UserName;
                    x.Email = customUser.Email;
                    await _userManager.UpdateAsync(x);
                    return Ok("You updated successfully!");
                }
            }
            return BadRequest("Dont have user in server");

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet("[controller]/[action]/{id}")]
        public async Task<ActionResult> UpdateRoll(string id)
        {
            var x = await _userManager.FindByIdAsync(id);
            var dsAllQuyen = await _roleManager.Roles.ToListAsync();
            var roleDTO = new RoleDTO();
            if (x != null)
            {
                var userRole = await _userManager.GetRolesAsync(x);
                roleDTO.UsersRole = userRole.ToList();
                roleDTO.ListRoles = dsAllQuyen;
            }
            return Ok(roleDTO);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<ActionResult> DoUpdateRoll(string userId, List<String> arrRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var dsAllQuyen = await _roleManager.Roles.ToListAsync();
            var arrQuyen = dsAllQuyen.Select(x => x.Name);
            if (user != null)
            {
                foreach (var quyen in arrQuyen)
                {
                    await _userManager.RemoveFromRoleAsync(user, quyen);
                }
                if (arrRole.Count > 0)
                {
                    foreach (var item in arrRole)
                    {
                        await _userManager.AddToRoleAsync(user, item);
                    }
                }
                return Ok("Authorizations successfully");
            }
            return BadRequest("Error in server");

        }

        [AllowAnonymous]
        [HttpGet("GetDetailStoriesWithUserId/{userId}")]
        public async Task<ActionResult<UserDTO>> GetDetailStoriesWithUserId(string userId)
        {
            var userDetail = await _userRepository.GetDetailStoriesWithUserId(userId);
            userDetail = await _storyRepository.GetAllStoriesViewByOneUserDTO(userDetail);
            if (userDetail == null)
            {
                return NotFound();
            }
            return Ok(userDetail);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "AUTHOR")]
        [HttpGet("GetDetailStoriesWithAuthorThemself")]
        public async Task<ActionResult<UserDTO>> GetDetailStoriesWithAuthorThemself()
        {
            string userId = String.Empty;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = identity.FindFirst(ClaimTypes.Sid).Value;
            }
            var storyDetaill = await _userRepository.GetDetailStoriesWithUserId(userId);
            var imageStoriess = await _storyRepository.GetAllStoriesViewByOneUserDTO(storyDetaill);
            if (imageStoriess == null)
            {
                return NotFound();
            }
            return Ok(imageStoriess);
        }
        [AllowAnonymous]
        [HttpGet("GetDetailStoriesWithAllUser")]
        public async Task<ActionResult<List<UserDTO>>> GetDetailStoriesWithAllUser()
        {
            var storyDetaill = await _userRepository.GetDetailStoriesWithAllUser();
            var imageStoriess = await _storyRepository.GetAllStoriesViewByUserDTO(storyDetaill);
            if (imageStoriess == null)
            {
                return NotFound();
            }
            return Ok(imageStoriess);
        }

    }
}
