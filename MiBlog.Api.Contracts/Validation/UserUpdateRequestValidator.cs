using FluentValidation;
using MiBlog.Api.Contracts.Models.Users;
using MyBlog.Domain.Interfaces;

namespace MiBlog.Api.Contracts.Validation
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        private readonly IUserRepository _userRepository;
        public UserUpdateRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.id)
                .NotEmpty().WithMessage("Поле не может быть пустым.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Поле не может быть пустым.")
                .Must(CheckEmail).WithMessage(x => $"Пользователь с Email: {x.Email} уже существует.")
                .EmailAddress().WithMessage("Требуется указать действительный адрес электронной почты.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Поле не может быть пустым.");
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Поле не может быть пустым.");
        }

        private bool CheckEmail(string email)
        {
            var num = _userRepository.GetAll().Where(u => u.Email == email);

            if (num.Count() > 1)
                return false;

            return true;
        }
    }
}
