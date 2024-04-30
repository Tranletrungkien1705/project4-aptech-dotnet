using System;

namespace Project4.DTO
{
    public class StoryUserDTO
    {
        public String? StoryId { get; set; }
        public String? Name { get; set; }
        public String? SubName { get; set; }
        public String? Description { get; set; }
        public String? Image { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public String? View { get; set; }
        public bool? Status { get; set; }
    }
}
