namespace Project4.DTO
{
    public class StoryChapterDTO
    {
        public String? Id { get; set; }
        public String? Name { get; set; }
        public String? SubName { get; set; }
        public String? Description { get; set; }
        public string? UpdateTime { get; set; }
        public int? ViewOfStory { get; set; }
        public List<chapterDTO> chapterDTOs { get; set; }
        public List<authorDTO> authorDTOs { get; set; }
        public List<cateDTO> cateDTOs { get; set; }
        public String? Image { get; set; }
        public bool? Status { get; set; }
    }
    public class chapterDTO
    {
        public String? chapterId { get; set; }
        public String? chapterName { get; set; }
        public int? view { get; set; }
    }
    public class cateDTO
    {
        public String? cateId { get; set; }
        public String? cateName { get; set; }
    }
    public class authorDTO
    {
        public String? userId { get; set; }
        public String? userName { get; set; }
    }
}