using AutoMapper;
using MiBlog.Api.Contracts.Models.Comments;
using MiBlog.Api.Contracts.Models.Info;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;

        public CommentController(IArticleRepository articleRepository,
                                 IUserRepository userRepository,
                                 IRepository<Comment> commentRepository,
                                 IMapper mapper)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить комментарий по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Id и текст комментария. Модель <a href='#model-CommentResponse'>CommentResponse</a></response>
        /// <response code="204">Комментарий с указанным ID не найден.</response>
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CommentResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]       
        public ActionResult Get(int id)
        {
            var comment = _commentRepository.Get(id);

            if(comment == null)
                return StatusCode(204, new { message = $"Комментарий с ID: {id} не найден." });

            var response = _mapper.Map<CommentResponse>(comment);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить все комментарии конкретной статьи.
        /// </summary>
        /// <param name="idArticle">ID статьи.</param>
        /// <response code="200">Комментарии к статье. Модель <a href='#model-CommentResponse'>CommentResponse</a></response>
        /// <response code="204">Комментарии к статье с указанным ID не найдены.</response>
        [HttpGet("{idArticle}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CommentResponse[]))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        public ActionResult GetAll(int idArticle)
        {
            var comments = _commentRepository.GetAll().Where(c => c.ArticleId == idArticle).OrderByDescending(c => c.Created);

            if (comments.Count() == 0)
                return StatusCode(204, new { message = $"К статье с ID: {idArticle} комментарии не найдены." });

            var response = _mapper.Map< Comment[], CommentResponse[]>(comments.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Создание нового комментария.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "idArticle": "1",
        ///        "content": "Очень интересная статья"
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-CommentCreateRequest'>CommentCreateRequest</a></param>
        /// <response code="201">Создание комментария прошло успешно. Модель <a href='#model-CommentResponse'>CommentResponse</a></response>
        /// <response code="204">Статья с указанным ID не найдена.</response>
        /// <response code="400">Текст комментария не может быть пустым.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(CommentResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Create(CommentCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Content))
                    return StatusCode(400, new { message = "Текст комментария не может быть пустым." });

                var article = _articleRepository.Get(request.IdArticle);

                if(article == null)
                    return StatusCode(204, new { message = $"Статья с ID: {request.IdArticle} не найдена." });

                var comment = new Comment();

                comment.Article = article;

                comment.User = _userRepository.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

                comment.Content = request.Content;

                comment.Created = DateTime.Now;

                _commentRepository.Create(comment);

                var response = _mapper.Map<CommentResponse>(comment);

                return StatusCode(201, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующего комментария.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///        "id": "1",
        ///        "content": "Очень интересная статья"
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-CommentUpdateRequest'>CommentUpdateRequest</a></param>
        /// <response code="200">Обновление прошло успешно.</response>
        /// <response code="204">Комментарий с указанным ID не найден.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Moderator".</response>
        [Route("Update")]        
        [HttpPut]
        [Authorize(Roles = "Moderator")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Update(CommentUpdateRequest request)
        {
            try
            {
                var comment = _commentRepository.Get(request.Id);

                if (comment == null)
                    return StatusCode(204, new { message = $"Комментарий с ID: {request.Id} не найден." });

                comment.Content = request.Content;

                _commentRepository.Update(comment);

                return StatusCode(200, "Комментарий обновлен.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Удаление комментария по id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Комментарий удалён.</response>   
        /// <response code="204">Комментарий с указанным ID не найден.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="Moderator".</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Moderator")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public ActionResult Delete(int id)
        {
            try
            {
                var comment = _commentRepository.Get(id);

                if (comment == null)
                    return StatusCode(204, new { message = $"Комментарий с ID: {id} не найден." });

                _commentRepository.Delete(comment);

                return StatusCode(200, "Комментарий удалён.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }
    }
}
