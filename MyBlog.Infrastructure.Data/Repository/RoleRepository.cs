using Microsoft.EntityFrameworkCore;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class RoleRepository : Repository<Role>
    {
        public RoleRepository(AppDbContext db) : base(db)
        { 
            
        }
    }
}

    