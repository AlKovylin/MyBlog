using MyBlog.Infrastructure.Business.Models;

namespace MiBlog.Api.Contracts.Models.Articles
{
    public class ArticleResponse
    {
        public ArticleModel? Article { get; set; }
        public UserModel? Author { get; set; }
        public CommentModel[]? Comments { get; set; }
        public TagModel[]? TagsArticle { get; set; }
    }
}
