using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class CommentsViewModel
    {
        public List<CommentModel> Comments = new List<CommentModel>();
    }
}
