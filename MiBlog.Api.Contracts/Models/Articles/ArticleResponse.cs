using MyBlog.Infrastructure.Business.Models;

namespace MiBlog.Api.Contracts.Models.Articles
{
    public class ArticleResponse
    {
        public ArticleModel? Article { get; set; }
        public UserModel? Author { get; set; }
        public List<CommentModel> Comments = new List<CommentModel>();
        public List<TagModel> TagsArticle = new List<TagModel>();
    }
}
