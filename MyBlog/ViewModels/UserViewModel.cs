using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.ViewModels
{
    public class UserViewModel
    {
        public int id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DisplayName { get; set; }
        public string AboutMy { get; set; }
        public string Photo { get; set; }
        public List<RoleModel> Roles = new List<RoleModel>();
        public List<RoleModel> AllRoles = new List<RoleModel>();
    }
}
