using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Comments
{
    public class CommentUpdateRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Content { get; set; }
    }
}
