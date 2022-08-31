using MyBlog.Infrastructure.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class ArticleCreateViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Published { get; set; } = DateTime.Now;
        public List<TagModel> Tags = new List<TagModel>();
        public List<TagModel> AllTags = new List<TagModel>();
    }
}
