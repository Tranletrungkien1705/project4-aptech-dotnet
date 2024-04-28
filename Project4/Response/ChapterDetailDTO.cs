namespace Project4.Response
{
    public class ChapterDetailDTO
    {
        public String? Id { get; set; }
        public String? Name { get; set; }
        public String? SubName { get; set; }
        public String? Content { get; set; }
        public DateTime? CreatedTime { get; set; } 
        public String? View { get; set; }
        public bool? IsPay { get; set; }
    }
}
