namespace Project4.DTO
{
    public class ContentDTO
    {
        public String? Id { get; set; }
        public String? ChapterName { get; set; }
        public String? ContentName { get; set; }
        public String? Images { get; set; }
        public List<PageeDTO> Page { get; set;} = new List<PageeDTO> { };

    }
}
