using MyBlog.Infrastructure.Business.Models;

namespace MiBlog.Api.Contracts.Models.Articles
{
    public class ArticleRequest
    {
        public ArticleModel? Article { get; set; }
        public TagModel[]? TagsArticle { get; set; }
    }
}
