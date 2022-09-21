using AutoMapper;
using MiBlog.Api.Contracts.Models.Info;
using MiBlog.Api.Contracts.Models.Role;
using MiBlog.Api.Contracts.Models.Roles;
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
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public RoleController(IRepository<Role> roleRepository, IMapper mapper)                                                                  
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить роль по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Id, имя и описание роли. Модель <a href='#model-RoleResponse'>RoleResponse</a></response>
        /// <response code="204">Роль с указанным ID не найдена.</response>
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        public ActionResult Get(int id)
        {
            var role = _roleRepository.Get(id);

            if (role == null)
                return StatusCode(204, new { message = $"Роль с ID: {id} не найдена." });

            var response = _mapper.Map<RoleResponse>(role);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить все роли.
        /// </summary>
        /// <response code="200">Роли пользователей. Модель <a href='#model-RoleResponse'>RoleResponse</a></response>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(RoleResponse[]))]
        public ActionResult GetAll()
        {
            var roles = _roleRepository.GetAll();

            var response = _mapper.Map<Role[], RoleResponse[]>(roles.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Создание новой роли.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "name": "User",
        ///        "description": "Базовая роль с минимальными правами."
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-RoleCreateRequest'>RoleCreateRequest</a></param>
        /// <response code="201">Создание роли прошло успешно. Модель <a href='#model-RoleResponse'>RoleResponse</a></response>
        /// <response code="400">Сообщение об ошибке валидации:
        /// <ul>
        ///     <li>"Имя роли не может быть пустым."</li>
        ///     <li>"Роль с именем: {Name} уже существует."</li>
        ///     <li>"Описание роли не может быть пустым."</li>
        /// </ul>
        /// </response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Admin".</response>
        [Route("Create")]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Create(RoleCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    return StatusCode(400, new { message = "Имя роли не может быть пустым." });
                
                var checkName = _roleRepository.GetAll().Any(t => t.Name == request.Name);

                if (checkName)
                    return StatusCode(400, new { message = $"Роль с именем: {request.Name} уже существует." });

                if (string.IsNullOrEmpty(request.Description))
                    return StatusCode(400, new { message = "Описание роли не может быть пустым." });

                var role = new Role();

                role.Name = request.Name;

                role.Description = request.Description;

                _roleRepository.Create(role);

                var response = _mapper.Map<RoleResponse>(role);

                return StatusCode(201, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующей роли.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "id": "1",
        ///        "description": "Базовая роль с минимальными правами."
        ///     }
        /// </remarks>
        /// <param name="request">Для редактирования доступно только описание роли. Модель <a href='#model-RoleUpdateRequest'>RoleUpdateRequest</a></param>
        /// <response code="200">Обновление прошло успешно. Модель <a href='#model-RoleResponse'>RoleResponse</a></response>
        /// <response code="204">Роль с указанным ID не найдена.</response>
        /// <response code="400">Описание роли не может быть пустым.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Admin".</response>
        [Route("Update")]
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Update(RoleUpdateRequest request)
        {
            try
            {
                var role = _roleRepository.Get(request.Id);

                if (role == null)
                    return StatusCode(204, new { message = $"Роль с ID: {request.Id} не найдена." });

                if (string.IsNullOrEmpty(request.Description))
                    return StatusCode(400, new { message = "Описание роли не может быть пустым." });

                role.Description = request.Description;

                _roleRepository.Update(role);

                var response = _mapper.Map<RoleResponse>(role);

                return StatusCode(200, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Удаление роли по id.
        /// </summary>
        /// <param name="id">Попытка удаления ролей с id = 1||2||3 вызовет ошибку 403.</param>
        /// <response code="200">Роль удалена.</response>
        /// <response code="400">Роль с ID: {id} не найдена.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Admin".</response>
        /// <response code="403">Роль {Name} является базовой и не может быть удалена.</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]        
        [SwaggerResponse(StatusCodes.Status403Forbidden, null, typeof(Message))]
        public ActionResult Delete(int id)
        {
            try
            {
                var role = _roleRepository.Get(id);

                if (role == null)
                    return StatusCode(400, new { message = $"Роль с ID: {id} не найдена." });

                if (role.Name == "User" || role.Name == "Moderator" || role.Name == "Admin")
                    return StatusCode(403, new { message = $"Роль {role.Name} является базовой и не может быть удалена." });

                _roleRepository.Delete(role);

                return StatusCode(200, "Роль удалена.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }
    }
}
