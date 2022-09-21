using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Users
{
    public class UserUpdateRequest
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? DisplayName { get; set; }
        public string? AboutMy { get; set; }
    }
}
