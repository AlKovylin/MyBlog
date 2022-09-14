using FluentValidation;
using MiBlog.Api.Contracts.Models.Users;
using MyBlog.Domain.Interfaces;

namespace MiBlog.Api.Contracts.Validation
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        private readonly IUserRepository _userRepository;

        public UserRegisterRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Поле не может быть пустым.")
                .Must(CheckEmail).WithMessage(x => $"Пользователь с Email: {x.Email} уже существует.")
                .EmailAddress().WithMessage("Требуется указать действительный адрес электронной почты.");
            RuleFor(x => x.RegPassword)
                .NotEmpty().WithMessage("Поле не может быть пустым.")
                .MinimumLength(5).WithMessage("Пароль должен содержать не менее 5-ти символов.");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Поле не может быть пустым.")
                .Equal(x => x.RegPassword).WithMessage("Пароли не совпадают.");
        }

        private bool CheckEmail(string email)
        {
            return !_userRepository.GetAll().Any(u => u.Email == email);               
        }
    }
}
