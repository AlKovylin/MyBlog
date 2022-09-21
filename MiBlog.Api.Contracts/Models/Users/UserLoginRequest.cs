using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Users
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string? Password { get; set; }
    }
}
