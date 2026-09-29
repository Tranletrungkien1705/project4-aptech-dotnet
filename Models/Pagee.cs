using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Pagee : Base
    {
        public String? Content { get; set; }
        public String? Images { get; set; }

        [ForeignKey(nameof(Chapter.Id))]
        public String? ChapterId { get; set; }
        public virtual Chapter? Chapters { get; set; }

    }
}
