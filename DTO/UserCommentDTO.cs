namespace Project4.DTO
{
    public class UserCommentDTO
    {
        public String? UserId {  get; set; }
        public List<CommentUserDTO> Comments { get; set; }
    }
}
