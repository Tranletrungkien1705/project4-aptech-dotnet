using Project4.Response;
using Project4.Models;


namespace Project4.DTO
{
    public class CategoryDTO
    {
        public String? Id { get; set; }
        public String? Name { get; set; }
        public String? Description { get; set; }

        public List<StoryResponse> Stories { get; set;} 

    }
}
