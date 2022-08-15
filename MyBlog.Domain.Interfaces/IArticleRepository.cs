using MyBlog.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Domain.Interfaces
{
    public interface IArticleRepository : IRepository<Article>
    {
        List<Tag> GetArticleTags(Article article);
    }    
}
