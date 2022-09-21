using AutoMapper;
using FluentValidation;
using MiBlog.Api.Contracts.Models.Info;
using MiBlog.Api.Contracts.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserUpdateRequest> _validatorUser;

        public UserController(IUserRepository userRepository, IMapper mapper, IValidator<UserUpdateRequest> validatorUser)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validatorUser = validatorUser;
        }

        /// <summary>
        /// Получить данные пользователя по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Данные пользователя. Модель <a href='#model-UserResponse'>UserResponse</a></response>
        /// <response code="204">Пользователь с ID: {id} не найден.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Get(int id)
        {
            var user = _userRepository.Get(id);

            if (user == null)
                return StatusCode(204, new { message = $"Пользователь с ID: {id} не найден." });

            var response = _mapper.Map<UserResponse>(user);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить данные всех пользователей.
        /// </summary>
        /// <response code="200">Зарегистрированные пользователи. Модель <a href='#model-UserResponse'>UserResponse</a></response>
        /// <response code="204">Нет ни одного зарегистрированного пользователя.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Admin".</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserResponse[]))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult GetAll()
        {
            var users = _userRepository.GetAll();

            if (users.Count() == 0)
                return StatusCode(204, new { message = "Нет ни одного зарегистрированного пользователя." });

            var response = _mapper.Map<User[], UserResponse[]>(users.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Обновление данных существующего пользователя.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "id": "1",
        ///        "email": "example@mail.ru",
        ///        "name": "Иван Иванов",
        ///        "displayName": "Обо всём по немногу.",//название блога
        ///        "aboutMy": "Продвинутый программист."
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-UserUpdateRequest'>UserUpdateRequest</a></param>
        /// <response code="200">Обновление прошло успешно. Модель <a href='#model-UserResponse'>UserResponse</a></response>
        /// <response code="204">Пользователь с ID: {id} не найден.</response>
        /// <response code="400">Сообщение об ошибке валидации введённых данных.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [Route("Update")]
        [HttpPut]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Update(UserUpdateRequest request)
        {
            try
            {
                var user = _userRepository.Get(request.id);

                if (user == null)
                    return StatusCode(204, new { message = $"Пользователь с ID: {request.id} не найден." });

                var validationResult = _validatorUser.Validate(request);

                if (!validationResult.IsValid)
                    return StatusCode(400, validationResult.Errors);

                user.Email = request.Email;

                user.Name = request.Name;

                user.DisplayName = request.DisplayName;

                user.AboutMy = request.AboutMy;

                _userRepository.Update(user);

                var response = _mapper.Map<UserResponse>(user);

                return StatusCode(200, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Удаление пользователя по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Пользователь удалён.</response>
        /// <response code="204">Пользователь с ID: {id} не найден.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Admin".</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Delete(int id)
        {
            try
            {
                var user = _userRepository.Get(id);

                if (user == null)
                    return StatusCode(204, new { message = $"Пользователь с ID: {id} не найден." });

                _userRepository.Delete(user);

                return StatusCode(200, "Пользователь удалён.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }
    }
}
