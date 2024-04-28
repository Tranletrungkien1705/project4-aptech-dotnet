using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class UserRefreshTokens : Base
    {

        [Required]
        [ForeignKey(nameof(CustomUser.Id))]
        public string UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public string RefreshToken { get; set; }

        public DateTime ExpiryDate { get; set; }
        public virtual CustomUser? CustomUsers { get; set; }
    }
}