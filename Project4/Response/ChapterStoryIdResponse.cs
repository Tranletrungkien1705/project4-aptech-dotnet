using Project4.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Response
{
    public class ChapterStoryIdResponse
    {
        public String? StoryId { get; set; }
        public String? StoryName { get; set; }

        public List<ChapterDetailDTO> Chapters { get; set; }
    }

    
}
