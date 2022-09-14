namespace MiBlog.Api.Contracts.Models.Users
{
    public class UserRegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? RegPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
