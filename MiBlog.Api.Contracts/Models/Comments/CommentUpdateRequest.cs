using MyBlog.Infrastructure.Business.Models;

namespace MiBlog.Api.Contracts.Models.Comments
{
    public class CommentUpdateRequest
    {
        public int Id { get; set; }
        public string? Content { get; set; }
    }
}
