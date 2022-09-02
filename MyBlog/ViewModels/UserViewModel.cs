using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class UserViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный формат. Пример: name@example.com")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Text)]
        public string DisplayName { get; set; }
        public string AboutMy { get; set; }
        public string Photo { get; set; }
        public List<RoleModel> Roles = new List<RoleModel>();
        public List<RoleModel> AllRoles = new List<RoleModel>();
    }
}
