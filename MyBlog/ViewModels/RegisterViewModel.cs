using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный формат. Пример: name@example.com")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]        
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Поле должно иметь не менее {2} и не более {1} символов.", MinimumLength = 5)]        
        public string RegPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("RegPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}