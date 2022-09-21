using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Role
{
    public class RoleUpdateRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}
