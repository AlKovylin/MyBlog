using MyBlog.Domain.Core;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class RoleRepository : Repository<Role>
    {
        public RoleRepository(AppDbContext db) : base(db)
        { 
            
        }
    }
}

    