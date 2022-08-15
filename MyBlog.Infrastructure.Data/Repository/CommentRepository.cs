using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class CommentRepository : Repository<Comment>
    {
        public CommentRepository(AppDbContext db) : base(db)
        { 
            
        }
    }
}
