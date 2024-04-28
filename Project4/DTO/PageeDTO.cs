using Project4.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.DTO
{
    public class PageeDTO
    {
        public String? Content { get; set; }

        public List<IFormFile>? Images { get; set; }
        public String? ChapterId { get; set; }
        public String? PageId { get; set; }
    }
}
