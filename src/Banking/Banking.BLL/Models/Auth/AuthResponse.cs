namespace Banking.BLL.Models.Auth
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
