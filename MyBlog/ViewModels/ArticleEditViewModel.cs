using MyBlog.Infrastructure.Business.Models;
using System;
using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class ArticleEditViewModel
    {
        public ArticleModel Article { get; set; }
        public List<TagModel> TagsArticle = new List<TagModel>();
        public List<TagModel> TagsAll = new List<TagModel>();
    }
}
