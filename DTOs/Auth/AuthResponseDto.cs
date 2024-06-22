namespace AuthenBackend.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpired { get; set; }
    }
}