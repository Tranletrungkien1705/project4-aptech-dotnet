using Project4.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Viewed : Base
    {
        [ForeignKey(nameof(Chapter.Id))]
        public String? ChapterId { get; set; }

        [ForeignKey(nameof(CustomUser.Id))]
        public String? UserId { get; set; }
        public virtual Chapter? Chapters { get; set; }
        public virtual CustomUser? CustomUsers { get; set; }
    }
}
