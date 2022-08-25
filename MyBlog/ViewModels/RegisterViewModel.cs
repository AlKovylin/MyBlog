using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(20, ErrorMessage = "Поле {0} должно иметь не менее {2} и не более {1} символов.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}

//[Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
//[DataType(DataType.Password)]
//[Display(Name = "Пароль", Prompt = "Введите пароль")]
//[StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
//public string PasswordReg { get; set; }

//[Required(ErrorMessage = "Обязательно подтвердите пароль")]
//[Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
//[DataType(DataType.Password)]
//[Display(Name = "Подтвердить пароль", Prompt = "Введите пароль повторно")] 