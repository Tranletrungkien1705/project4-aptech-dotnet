using Project4.Response;

namespace Project4.DTO
{
    public class ChapterDTO
    {
        public String? ChapterId { get; set; }
        //public String? StoryId { get; set; }
        public String? StoryName { get; set; }
        public String? Content { get; set; }
        public String? ChapterName { get; set; }
        public String? SubName { get; set; }
        public String? Description { get; set; }
        public string PageId { get; set; }
        public List<Images>? Images { get; set; }
        public List<Commentt>? Commentts { get; set; }
        public List<ChapterResponse> ImagesWithChapter { get; set; }
    }
    public class Images
    {
        public String ImageId { get; set; }
        public IFormFile? ImageLink { get; set; }
    }

    public class Commentt
    {
        public String? Id { get; set; }  // Unique identifier for each comment
        public String? userAvatarUrl { get; set; } // URL for the user's avatar image
        public String? UserId { get; set; }
        public String? UserName { get; set; }// User's name
        public String? Content { get; set; } // Comment text
        public List<Commentt> replies { get; set; }
        public String? CommentsId { get; set; }
    }
}
