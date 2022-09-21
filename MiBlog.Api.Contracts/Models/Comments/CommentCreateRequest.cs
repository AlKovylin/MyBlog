using System.ComponentModel.DataAnnotations;

namespace MiBlog.Api.Contracts.Models.Comments
{
    public class CommentCreateRequest
    {
        [Required]
        public int IdArticle { get; set; }
        [Required]
        public string? Content { get; set; }
    }
}
