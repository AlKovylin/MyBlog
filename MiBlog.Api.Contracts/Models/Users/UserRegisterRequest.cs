using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Users
{
    public class UserRegisterRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string? RegPassword { get; set; }
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
