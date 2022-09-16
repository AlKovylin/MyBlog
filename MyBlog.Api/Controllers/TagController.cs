using AutoMapper;
using MiBlog.Api.Contracts.Models.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]    
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
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        public ActionResult Get(int id)
        {
            var tag = _tagRepository.Get(id);

            if (tag == null)
                return StatusCode(400, new { message = $"Тег с ID: {id} не найден." });

            var response = _mapper.Map<TagResponse>(tag);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить все теги.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult GetAll()
        {
            var tags = _tagRepository.GetAll().OrderBy(t => t.Name);

            if (!tags.Any())
                return StatusCode(400, new { message = "Теги не созданы." });

            var response = _mapper.Map<Tag[], TagResponse[]>(tags.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Создание нового тега.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Созданный тег.</returns>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "Moderator")]
        //[ValidateAntiForgeryToken]
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

                return StatusCode(200, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующего тега.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Обновлённый тег.</returns>
        [Route("Update")]        
        [HttpPost]
        [Authorize(Roles = "Moderator")]
        //[ValidateAntiForgeryToken]
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
                    return StatusCode(400, new { message = $"Тег с ID: {request.Id} не найден." });

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
        /// <returns>Сообщение о результате выполнения операции.</returns>        
        [HttpDelete("id")]
        [Authorize(Roles = "Moderator")]
        //[ValidateAntiForgeryToken]
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
