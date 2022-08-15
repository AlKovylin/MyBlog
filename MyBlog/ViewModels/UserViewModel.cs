using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.ViewModels
{
    public class UserViewModel
    {
        public UserModel User { get; set; }
        public List<ArticleModel> Articles { get; set; }        
    }
}
