using Project4.DTO;
using Project4.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Response
{
    public class ChapterResponse
    {
        public String? StoryIdChapter { get; set; }
        public String? ChapterId { get; set; }
        public String? ChapterName { get; set; }
        public String? ChapterContent { get; set; }
        public List<PageeResponse> Pages { get; set; }
    }
}
