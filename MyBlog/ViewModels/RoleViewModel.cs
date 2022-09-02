using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указан название роли")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Добавьте описание роли")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
