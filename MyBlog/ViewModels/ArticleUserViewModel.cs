using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class ArticleUserViewModel
    {
        public ArticleModel Article { get; set; }
        public List<TagModel> Tags = new List<TagModel>();
    }
}
