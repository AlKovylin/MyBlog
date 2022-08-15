using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class TagRepository : Repository<Tag>
    {
        public TagRepository(AppDbContext db) : base(db)
        { 
            
        }
    }
}
