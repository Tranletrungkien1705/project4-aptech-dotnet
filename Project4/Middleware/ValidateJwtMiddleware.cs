using Project4.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace database2.Middleware
{
    public class ValidateJwtMiddleware
        {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _tokenValidation;

        public ValidateJwtMiddleware(RequestDelegate next, TokenValidationParameters tokenValidation)
        {
            _next = next;
            _tokenValidation = tokenValidation;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var tokenValid = ValidateToken(context,token);
                if (!tokenValid)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }

            await _next(context);
        }

        private bool ValidateToken(HttpContext context,string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidation.ValidateLifetime = true;
                var tokenVerification = jwtTokenHandler.ValidateToken(token, _tokenValidation, out SecurityToken validateToken);
                if (validateToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return false;
                    }
                }
                var utvExpiryDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimmeStampToDatetime(utvExpiryDate);
                if (expiryDate > DateTime.Now)
                {
                    return false;
                }
                var userId = tokenVerification.Claims.First(x => x.Type == ClaimTypes.Sid).Value;
                // Attach user id to context on successful validation
                context.Items["UserId"] = userId;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            //return false; // Simulate an expired token
        }

        private string RefreshToken(string token)
        {
            // Logic to refresh the token
            return "newToken1234"; // Simulate a new token
        }
        private DateTime UnixTimmeStampToDatetime(long utvExpiryDate)
        {
            var datetimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            datetimeVal = datetimeVal.AddSeconds(utvExpiryDate).ToUniversalTime();
            return datetimeVal;
        }
    }
}
