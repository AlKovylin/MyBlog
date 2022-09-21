using AutoMapper;
using MiBlog.Api.Contracts.Models.Info;
using MiBlog.Api.Contracts.Models.Tags;
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
    public class TagController : ControllerBase
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public TagController(IRepository<Tag> tagRepository, IMapper mapper)
                                 
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить тег по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Id и имя тега. Модель <a href='#model-TagResponse'>TagResponse</a></response>
        /// <response code="204">Тег с указанным ID не найден.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(TagResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Get(int id)
        {
            var tag = _tagRepository.Get(id);

            if (tag == null)
                return StatusCode(204, new { message = $"Тег с ID: {id} не найден." });

            var response = _mapper.Map<TagResponse>(tag);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить все теги.
        /// </summary>
        /// <response code="200">Теги. Модель <a href='#model-TagResponse'>TagResponse</a></response>
        /// <response code="204">Нет ни одного тега.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [HttpGet]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(TagResponse[]))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult GetAll()
        {
            var tags = _tagRepository.GetAll().OrderBy(t => t.Name);

            if (!tags.Any())
                return StatusCode(204, new { message = "Нет ни одного тега." });

            var response = _mapper.Map<Tag[], TagResponse[]>(tags.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Создание нового тега.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "name": "#ASP.Net"
        ///     }
        /// </remarks>
        /// <param name="name">Имя тега должно начинаться с символа '#'</param>
        /// <response code="201">Создание тега прошло успешно. Модель <a href='#model-TagResponse'>TagResponse</a></response>
        /// <response code="400">Сообщение об ошибке валидации:
        /// <ul>
        ///     <li>"Имя тега не может быть пустым."</li>
        ///     <li>"Имя тега должно начинаться с символа '#'."</li>
        ///     <li>"Тег с именем: {Name} уже существует."</li>
        /// </ul>
        /// </response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Moderator".</response>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "Moderator")]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(TagResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Create(string name)
        {
            try
            {                
                if(string.IsNullOrEmpty(name))
                    return StatusCode(400, new { message = "Имя тега не может быть пустым." });

                if (!name.StartsWith('#'))
                    return StatusCode(400, new { message = "Имя тега должно начинаться с символа '#'." });

                var checkName = _tagRepository.GetAll().Any(t => t.Name == name);

                if (checkName)
                    return StatusCode(400, new { message = $"Тег с именем: {name} уже существует." });                

                var tag = new Tag();

                tag.Name = name;

                _tagRepository.Create(tag);

                var response = _mapper.Map<TagResponse>(tag);

                return StatusCode(201, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующего тега.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "name": "#ASP.Net"
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-TagUpdateRequest'>TagUpdateRequest</a></param>
        /// <response code="200">Обновление тега прошло успешно. Модель <a href='#model-TagResponse'>TagResponse</a></response>
        /// <response code="204">Роль с указанным ID не найдена.</response>
        /// <response code="400">Сообщение об ошибке валидации:
        /// <ul>
        ///     <li>"Имя тега не может быть пустым."</li>
        ///     <li>"Имя тега должно начинаться с символа '#'."</li>
        ///     <li>"Тег с ID: {Id} не найден."</li>
        ///     <li>"Тег с именем: {Name} уже существует."</li>
        /// </ul>
        /// </response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Moderator".</response>
        [Route("Update")]        
        [HttpPut]
        [Authorize(Roles = "Moderator")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(TagResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Update(TagUpdateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    return StatusCode(400, new { message = "Имя тега не может быть пустым." });

                if (!request.Name.StartsWith('#'))
                    return StatusCode(400, new { message = "Имя тега должно начинаться с символа '#'." });

                var tag = _tagRepository.Get(request.Id);

                if (tag == null)
                    return StatusCode(204, new { message = $"Тег с ID: {request.Id} не найден." });

                var checkName = _tagRepository.GetAll().Any(t => t.Name == request.Name);

                if (checkName)
                    return StatusCode(400, new { message = $"Тег с именем: {request.Name} уже существует." });
               
                tag.Name = request.Name;

                _tagRepository.Update(tag);

                var response = _mapper.Map<TagResponse>(tag);

                return StatusCode(200, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Удаление тега по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Тег удалён.</response>
        /// <response code="400">Тег с ID: {id} не найден.</response>    
        /// <response code="401">Unauthorized: доступно пользователям Roles="Moderator".</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Moderator")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]        
        public ActionResult Delete(int id)
        {
            try
            {
                var tag = _tagRepository.Get(id);

                if (tag == null)
                    return StatusCode(400, new { message = $"Тег с ID: {id} не найден." });

                _tagRepository.Delete(tag);

                return StatusCode(200, "Тег удалён.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }
    }
}
