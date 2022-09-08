using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class ArticleViewModel
    {
        public ArticleModel Article { get; set; }
        public UserModel Author { get; set; }
        public List<CommentModel> Comments = new List<CommentModel>();        
        public List<TagModel> TagsArticle = new List<TagModel>();
    }
}
