namespace MiBlog.Api.Contracts.Models.Users
{
    public class UserLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}
