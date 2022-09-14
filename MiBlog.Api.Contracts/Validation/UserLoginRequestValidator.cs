using FluentValidation;
using MiBlog.Api.Contracts.Models.Users;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBlog.Api.Contracts.Validation
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        private readonly IUserRepository _userRepository;

        User? user = null;

        public UserLoginRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Поле не может быть пустым.")
                .Must(CheckEmail).WithMessage(x => $"Пользователь с Email: {x.Email} не существует.")
                .EmailAddress().WithMessage("Требуется указать действительный адрес электронной почты.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Поле не может быть пустым.")
                .Must(CheckPass).WithMessage("Пароль не совпадает.")
                .MinimumLength(5).WithMessage("Пароль должен содержать не менее 5-ти символов.");
        }

        private bool CheckEmail(string email)
        {
            user = _userRepository.GetAll().FirstOrDefault(u => u.Email == email);

            return user == null ? false : true;
        }

        private bool CheckPass(string? pass)
        {
            return user?.Password == pass ? true : false;
        }
    }
}
