using AutoMapper;
using FluentValidation;
using MiBlog.Api.Contracts.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserUpdateRequest> _validatorUser;

        public UserController(IArticleRepository articleRepository,
                                 IUserRepository userRepository,
                                 IRepository<Comment> commentRepository,
                                 IRepository<Tag> tagRepository,
                                 IMapper mapper,
                                 IValidator<UserUpdateRequest> validatorUser)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
            _validatorUser = validatorUser;
        }

        /// <summary>
        /// Получить данные пользователя по id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        public ActionResult Get(int id)
        {
            var user = _userRepository.Get(id);

            if (user == null)
                return StatusCode(400, new { message = $"Пользователь с ID: {id} не найден." });

            var response = _mapper.Map<UserResponse>(user);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить данные всех пользователей.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult GetAll()
        {
            var users = _userRepository.GetAll();

            if (users.Count() == 0)
                return StatusCode(400, new { message = "Нет ни одного зарегистрированного пользователя." });

            var response = _mapper.Map<User[], UserResponse[]>(users.ToArray());

            return StatusCode(200, response);
        }        

        /// <summary>
        /// Обновление данных существующего пользователя.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Обновлённые данные.</returns>
        [Route("Update")]
        [HttpPost]
        [Authorize(Roles = "User")]
        //[ValidateAntiForgeryToken]
        public ActionResult Update(UserUpdateRequest request)
        {
            try
            {
                var user = _userRepository.Get(request.id);

                if(user == null)
                    return StatusCode(400, new { message = $"Пользователь с ID: {request.id} не найден." });

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
        /// <returns>Сообщение о результате выполнения операции.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("id")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var user = _userRepository.Get(id);

                if (user == null)
                    return StatusCode(400, new { message = $"Пользователь с ID: {id} не найден." });

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
