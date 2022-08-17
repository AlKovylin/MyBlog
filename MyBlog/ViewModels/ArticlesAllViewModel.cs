using System.Collections.Generic;

namespace MyBlog.ViewModels
{
    public class ArticlesAllViewModel<T>
    {
        public List<T> AllArticles = new List<T>();
    }
}
