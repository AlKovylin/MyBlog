using MyBlog.Domain.Core;
using System;
using System.Collections.Generic;

namespace MyBlog.Services.Interfaces
{
    public interface IUserService : IService<User>
    {        
        bool Check(string email);
        User GetByLogin(string login);
        List<Role> GetUserRoles(User user);
    }
}
