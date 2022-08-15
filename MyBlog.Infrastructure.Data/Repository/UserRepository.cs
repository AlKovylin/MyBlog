using Microsoft.EntityFrameworkCore;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db)
        { 
            
        }

        public List<Role> GetUserRoles(User user)
        {
            return _db.Roles.Include(r => r.User).Where(r => r.User.Contains(user)).ToList();            
        }

        public bool Check(string email)
        {
            return GetAll().Any(u => u.Email == email);
        }
    }
}
