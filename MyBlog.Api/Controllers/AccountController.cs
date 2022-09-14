using AutoMapper;
using FluentValidation;
using MiBlog.Api.Contracts.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserRegisterRequest> _validatorRegister;
        private readonly IValidator<UserLoginRequest> _validatorLogin;

        public AccountController(IUserRepository userRepository, 
                                 IRepository<Role> roleRepository, 
                                 IMapper mapper, 
                                 IValidator<UserRegisterRequest> validatorRegister,
                                 IValidator<UserLoginRequest> validatorLogin)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _validatorRegister = validatorRegister;
            _validatorLogin = validatorLogin;
        }

        /// <summary>
        /// Обеспечивает вход в систему
        /// </summary>  
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Authenticate(UserLoginRequest request)
        {
            var validationResult = _validatorLogin.Validate(request);

            if (!validationResult.IsValid)
                return StatusCode(400, validationResult.Errors);

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == request.Email);

            await Authenticate(user);

            var response = _mapper.Map<UserLoginResponse>(user);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var validationResult = _validatorRegister.Validate(request);

            if (!validationResult.IsValid)
                return StatusCode(400, validationResult.Errors);

            var user = _mapper.Map<User>(request);

            user.Role.Add(_roleRepository.GetAll().FirstOrDefault(r => r.Name == DefaultRole.Role));

            _userRepository.Create(user);

            await Authenticate(user);

            var response = _mapper.Map<UserLoginResponse>(user);

            return StatusCode(201, response);
        }

        /// <summary>
        /// Регистрирует пользователя в системе
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task Authenticate(User user)
        {
            var userRoles = _userRepository.GetUserRoles(user);

            //создаем claim на основе e-mail
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
            };

            //добавляем клаймы на основе ролей пользователя
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "AppCookie");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);//устанавливаем куки для пользователя
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return StatusCode(200, "Выход из системы выполнен.");
        }
    }
}
