using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Story_Category : Base
    {
        [ForeignKey(nameof(Category.Id))]
        public String? CategoryId { get; set; }

        [ForeignKey(nameof(Story.Id))]
        public String? StoryId { get; set; }
        public virtual Category? Categories { get; set; }
        public virtual Story? Stories { get; set; }
    }
}
