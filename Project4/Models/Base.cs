using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Models
{
    public class Base
    {
        [Key]
        public String Id { get; set; }
        public String? CreatedUser { get; set; }
        public DateTime? CreatedTime { get; set; }
        public String? UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool? IsDeleted { get; set; }
        public String? DeletedUser { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}
