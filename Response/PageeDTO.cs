using Project4.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project4.Response
{
    public class PageeResponse
    {
        public String? Content { get; set; }

        public String? Images { get; set; }
        public String? ChapterId { get; set; }
        public String? PageId { get; set; }
    }
}
