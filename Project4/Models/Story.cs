using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Story : Base 
    {
        public String? Name { get; set; }
        public String? SubName { get; set; }
        public String? Description { get; set; }
        public String? Image { get; set; }
        public bool? Status { get; set; }

        [ForeignKey(nameof(CustomUser.Id))]
        public String? CustomUserId { get; set; }
        public virtual CustomUser? CustomUsers { get; set; }

    }
}
