namespace Project4.Response
{
    public class StoryResponse
    {
        public String? Id { get; set; }
        public String? Name { get; set; }
        public String? SubName { get; set; }
        //public String? UserName { get; set; }
        public Author? Author { get; set; } 
        public List<StoryCategory>? StoryCategory { get; set; }
        public String? Description { get; set; }
        public String? Image { get; set; }
        public String? View { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool? Status { get; set; }
    }
    public class Author
    {
        public String? UserId { get; set; }
        public String? UserName { get; set; }
    }
    public class StoryCategory
    {
        public String? CateId { get; set; }
        public String? CateName { get; set; }
    }
}
