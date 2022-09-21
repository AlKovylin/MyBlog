using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Role
{
    public class RoleCreateRequest
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}
