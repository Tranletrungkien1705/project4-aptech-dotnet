using Project4.Models;

namespace Project4.DTO
{
    public class UserDTO
    {
        public String? UserId { get; set; }
        public String? Email { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Position { get; set; }
        public List<StoryUserDTO> Stories { get; set; }
    }
}
