using MyBlog.Infrastructure.Business.Models;
using System;
using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class ArticleEditViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Modified { get; set; } = DateTime.Now;
        public List<TagModel> Tags = new List<TagModel>();
    }
}
