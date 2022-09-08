using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class ArticlesAllViewModel<T>
    {
        public List<T> AllArticles = new List<T>();
        public List<TagModel> AllTags = new List<TagModel>();
    }
}
