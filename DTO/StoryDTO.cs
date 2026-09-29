namespace Project4.DTO
{
    public class StoryDTO
    {
        public String? Id { get; set; }
        public String? Name { get; set; }
        public String? SubName { get; set; }
        public String? Description { get; set; }
        public IFormFile? Image { get; set;}
        public bool? Status { get; set; }
        public List<CateDTO> cateDTOs { get; set; }
    }
    public class CateDTO
    {
        public String cateId { get; set; }
        public String? cateName { get; set; }
    }
}
