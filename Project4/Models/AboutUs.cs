using System.ComponentModel.DataAnnotations;
namespace Project4.Models
{
    public class AboutUs
    {
        [Key]
        public String Id { get; set; }
        public String? Name { get; set; }
        public String? Desciption { get; set; }
        public String? Contact { get; set; }
    }
}
