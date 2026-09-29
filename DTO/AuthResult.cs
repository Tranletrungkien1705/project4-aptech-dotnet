namespace Project4.DTO
{
    public class AuthResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Result { get; set; }
        public List<String> Errors { get; set; }
    }
}
