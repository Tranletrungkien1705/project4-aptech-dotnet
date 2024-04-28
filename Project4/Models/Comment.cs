using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Comment: Base
    {
        public String? Content { get; set; }
        [ForeignKey(nameof(Comment.Id))]
        public String? CommentsId { get; set;}
        [ForeignKey(nameof(Chapter.Id))]
        public String? ChapterId { get; set; }
        public String? Feeling { get; set; }
        public virtual Comment? Comments { get; set; }
    }
}
