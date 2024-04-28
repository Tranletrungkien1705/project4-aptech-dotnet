using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Project4.Repository;
using Project4.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Project4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IJWTManagerRepository _jWTManager;

        public AuthController(IConfiguration configuration, UserManager<CustomUser> userManager, IJWTManagerRepository jWTManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _jWTManager = jWTManager;
        }

        [AllowAnonymous]
        [HttpPost("doLogin")]
        public async Task<IActionResult> DoLogin([FromBody] loginUser model)
        {
            try
            {
                //byte[] emailBytes = Convert.FromBase64String(model.Email);
                //byte[] PassBytes = Convert.FromBase64String(model.Password);
                //string email = Encoding.UTF8.GetString(emailBytes);
                //string password = Encoding.UTF8.GetString(PassBytes);
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                    return BadRequest(
                        new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>(){
                            "Invalid username or password"
                        }
                        });

                var checker = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!checker)
                    return BadRequest(
                        new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>(){
                            "Invalid username or password"
                        }
                        });

                var tokenString = await _jWTManager.GenerateToken(user.UserName);
                //TokenDTO response = new TokenDTO
                //{
                //    AccessToken = tokenString.AccessToken,
                //    RefreshToken = tokenString.RefreshToken
                //};
                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return StatusCode(500,
                        new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>(){
                            "An error occurred while processing your request"
                        }
                        });
            }
        }

        [AllowAnonymous]
        [HttpPost("doRegister")]
        public async Task<IActionResult> DoRegister([FromBody] loginUser model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                    return BadRequest(
                        new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>(){
                            "User already exists"
                        }
                        });

                var newUser = new CustomUser(model.Email);
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>(){
                            "Failed to register user"
                        }
                    });

                var tokenString = await _jWTManager.GenerateToken(newUser.UserName);
                var response = new TokenDTO
                {
                    AccessToken = tokenString.AccessToken,
                    RefreshToken = tokenString.RefreshToken
                };

                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Registration failed: {ex.Message}");
                return StatusCode(500,
                        new AuthResult()
                        {
                            Result = true,
                            Errors = new List<string>(){
                            "An error occurred while processing your request"
                        }
                        });
            }
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> refreshToken([FromBody] TokenDTO tokenDTO)
        {
            if (ModelState.IsValid)
            {
                var t = await _jWTManager.RefreshToken(tokenDTO);
                return Ok(t);
            }

            return Unauthorized();
        }

    }
}
