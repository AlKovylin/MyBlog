using AutoMapper;
using MiBlog.Api.Contracts.Models.Role;
using MiBlog.Api.Contracts.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var role = _roleRepository.Get(id);

            if (role == null)
                return StatusCode(400, new { message = $"Роль с ID: {id} не найдена." });

            var response = _mapper.Map<RoleResponse>(role);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить все роли.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAll()
        {
            var roles = _roleRepository.GetAll();

            if (!roles.Any())
                return StatusCode(400, new { message = "Теги не созданы." });

            var response = _mapper.Map<Role[], RoleResponse[]>(roles.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Создание новой роли.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Созданная роль.</returns>
        [Route("Create")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

                return StatusCode(200, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующей роли.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Обновлённую роль.</returns>
        [Route("Update")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Update(RoleUpdateRequest request)
        {
            try
            {
                var role = _roleRepository.Get(request.Id);

                if (role == null)
                    return StatusCode(400, new { message = $"Роль с ID: {request.Id} не найдена." });

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
        /// <param name="id"></param>
        /// <returns>Сообщение о результате выполнения операции.</returns>
        [HttpDelete("id")]
        //[ValidateAntiForgeryToken]
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
