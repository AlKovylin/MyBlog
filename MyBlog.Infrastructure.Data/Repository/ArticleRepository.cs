using Microsoft.EntityFrameworkCore;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(AppDbContext db) : base(db)
        { 
            
        }

        public List<Tag> GetArticleTags(Article article)
        {
            return _db.Tags.Include(r => r.Articles).Where(r => r.Articles.Contains(article)).ToList();
        }
    }
}
