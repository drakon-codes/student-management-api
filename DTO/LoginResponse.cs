namespace StudentManagementAPI.DTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public int AdminUserId { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
