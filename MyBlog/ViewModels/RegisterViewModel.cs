using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Поле {0} должно иметь не менее {2} и не более {1} символов.", MinimumLength = 5)]        
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]        
        public string ConfirmPassword { get; set; }
    }
}