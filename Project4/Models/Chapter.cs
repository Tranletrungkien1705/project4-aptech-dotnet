using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Chapter: Base
    {
        public String? Name { get; set; }
        public String? SubName { get; set; }
        public String? Content { get; set; }

        [ForeignKey(nameof(Story.Id))]
        public String? StoryId { get; set; }
        public bool? IsPay { get; set; }
        public virtual Story? Stories { get; set; }
    }
}
