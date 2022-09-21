using AutoMapper;
using FluentValidation;
using MiBlog.Api.Contracts.Models.Info;
using MiBlog.Api.Contracts.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Data;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
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
        /// Вход в систему.
        /// </summary>  
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "email": example@mail.ru,
        ///        "password": "*****"
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-UserLoginRequest'>UserLoginRequest</a></param>
        /// <returns></returns>
        /// <response code="201">Возвращает данные авторизованного пользователя. Модель <a href='#model-UserLoginResponse'>UserLoginResponse</a></response>
        /// <response code="400">Сообщение об ошибке валидации:
        /// <ul>
        ///     <li>"Поле не может быть пустым."</li>
        ///     <li>"Пользователь с Email: {x.Email} не существует."</li>
        ///     <li>"Требуется указать действительный адрес электронной почты."</li>
        ///     <li>"Пароль не совпадает."</li>
        ///     <li>"Пароль должен содержать не менее 5-ти символов."</li>
        /// </ul>
        /// </response>

        [HttpPost]
        [Route("Login")]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(UserLoginResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        public async Task<IActionResult> Authenticate(UserLoginRequest request)
        {
            var validationResult = _validatorLogin.Validate(request);

            if (!validationResult.IsValid)
                return StatusCode(400, validationResult.Errors);

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == request.Email);

            await Authenticate(user);

            var response = _mapper.Map<UserLoginResponse>(user);

            return StatusCode(201, response);
        }

        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "email": "example@mail.ru",
        ///        "regPassword": "*****",
        ///        "confirmPassword": "*****"
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-UserRegisterRequest'>UserRegisterRequest</a></param>
        /// <returns>Регистарционные данные.</returns>
        /// <returns></returns>
        /// <response code="201">Возвращает данные авторизованного пользователя. Модель <a href='#model-UserLoginResponse'>UserLoginResponse</a></response>
        /// <response code="400">Сообщение об ошибке валидации.       
        /// <ul>
        ///     <li>"Поле не может быть пустым."</li>
        ///     <li>"Пользователь с Email: {x.Email} уже существует."</li>
        ///     <li>"Требуется указать действительный адрес электронной почты."</li>
        ///     <li>"Пароль должен содержать не менее 5-ти символов."</li>
        ///     <li>"Пароли не совпадают."</li>
        /// </ul>
        /// </response>
        [HttpPost]
        [Route("Register")]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(UserLoginResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
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
        /// Регистрирует пользователя в системе.
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
        /// Выход из системы.
        /// </summary>
        /// <returns>Сообщение о результате выполнения операции.</returns>
        /// <response code="200">Выход из системы выполнен.</response>
        [HttpPost]
        [Route("Logout")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return StatusCode(200, "Выход из системы выполнен.");
        }
    }
}
