using MyBlog.Domain.Core;
using System.Collections.Generic;

namespace MyBlog.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        List<Role> GetUserRoles(User user);
        bool Check(string email);
    }    
}
