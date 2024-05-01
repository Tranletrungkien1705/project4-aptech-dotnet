using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Project4.Repository
{
    public interface IJWTManagerRepository
    {
        Task<AuthResult> GenerateToken(string userName);
        Task<AuthResult> GenerateRefreshToken(string userName);
        //Task<bool> HasRole(string user,string role);
        Task<AuthResult> RefreshToken(TokenDTO tokenDTO);
        Task<List<CustomUser>> GetAllCustomUser();
    }
    public class JWTManagerRepository :  IJWTManagerRepository
    {
        private readonly IConfiguration _iconfiguration;
        private readonly UserManager<CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly TokenValidationParameters _tokenValidation;

        public JWTManagerRepository(IConfiguration iconfiguration, UserManager<CustomUser> userManager, 
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context,
            TokenValidationParameters tokenValidation)
        {
            _iconfiguration = iconfiguration;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _tokenValidation = tokenValidation;
        }
        public async Task<AuthResult> GenerateToken(string userName)
        {
            return await GenerateJWTTokens(userName);
        }

        public async Task<AuthResult> GenerateRefreshToken(string username)
        {
            return await GenerateJWTTokens(username);
        }

        public async Task<AuthResult> GenerateJWTTokens(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return null;
                }
                var userId = user.Id;

                // Lấy danh sách các vai trò của người dùng
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Sid, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                };

                // Thêm các claim về vai trò vào danh sách các claim
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_iconfiguration["JWT:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddSeconds(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                    };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwttoken = tokenHandler.WriteToken(token);
                String refreshToken = GenerateRefreshToken();
                var userRefreshToken = new UserRefreshTokens()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefreshToken = refreshToken,
                    UserId = userId,
                    CreatedTime = DateTime.Now,
                    ExpiryDate = DateTime.UtcNow.AddMonths(2),
                    JwtId = token.Id,
                    Token = jwttoken,
                    IsRevoked= false,
                    IsUsed = false,
                };
                await _context.UserRefreshTokens.AddAsync(userRefreshToken);
                await _context.SaveChangesAsync();

                return new AuthResult { AccessToken =jwttoken, RefreshToken = refreshToken,Result=true };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public async Task<AuthResult> RefreshToken(TokenDTO tokenDTO) {
            if (tokenDTO.AccessToken != null || tokenDTO.RefreshToken != null) {
                var result = await VerifyAndGenerateToken(tokenDTO);
                if(result == null) 
                { 
                    return new AuthResult() { 
                        Errors = new List<string>(){ 
                            "Invalid tokens"
                        }
                    }; 
                }
                return result;
            }
            return new AuthResult()
            {
                Errors = new List<string>(){
                    "Invalid parameters"
                },
                Result = false
            };
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenDTO tokenDTO)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try {
                _tokenValidation.ValidateLifetime = false;
                var tokenVerification = jwtTokenHandler.ValidateToken(tokenDTO.AccessToken, _tokenValidation, out SecurityToken validateToken);
                if (validateToken is JwtSecurityToken jwtSecurityToken) {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                var utvExpiryDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimmeStampToDatetime(utvExpiryDate);
                if (expiryDate > DateTime.Now) {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Expired Token"
                        }
                    };
                }
                var storedToken = await _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.RefreshToken == tokenDTO.RefreshToken);
                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "You need to login again,beacause you never login."
                        }
                    };
                }
                if (storedToken.IsUsed == true)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "You need to login again,becaus it is used."
                        }
                    };
                }
                if (storedToken.IsRevoked == true)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "You need to login again.Because it is revoked."
                        }
                    };
                }
                var jti = tokenVerification.Claims.FirstOrDefault(x=>x.Type==JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "You need to login again.Because token is wrong."
                        }
                    };
                }
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Expired Token"
                        }
                    };
                }
                storedToken.IsUsed = true;
                _context.UserRefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJWTTokens(dbUser.UserName);
            }
            catch (Exception e) {
                return new AuthResult()
                {
                    Errors = new List<string>(){
                    "Server Error"
                },
                    Result = false
                };
            }
        }

        private DateTime UnixTimmeStampToDatetime(long utvExpiryDate)
        {
            var datetimeVal = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            datetimeVal.AddSeconds(utvExpiryDate).ToUniversalTime();
            return datetimeVal;
        }

        public async Task<List<CustomUser>> GetAllCustomUser()
        {
            var query = from st in _context.CustomUsers.AsQueryable()
                        select st;

            return await query.ToListAsync();
            return null;
        }
    }
}
