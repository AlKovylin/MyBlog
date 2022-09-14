using MyBlog.Infrastructure.Business.Models;

namespace MiBlog.Api.Contracts.Models.Articles
{
    public class ArticlesResponse
    {
        public List<ArticleResponse> Articles = new();
    }
}
