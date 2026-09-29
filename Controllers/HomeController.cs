using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Project4.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;
using System.Net.Mail;
namespace Project4.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private readonly SmtpClient _smtpClient;
        public HomeController(IHttpContextAccessor httpContextAccessor, UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Roling()
        {
            var x = new DbSeedRole(_roleManager);
            await x.RoleData();
            return Ok("SUcced");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    
}