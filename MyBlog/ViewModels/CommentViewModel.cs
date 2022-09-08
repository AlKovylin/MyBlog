using MyBlog.Infrastructure.Business.Models;
using System;

namespace MyBlog.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        //public int ArticleId { get; set; }
    }
}
