using AutoMapper;
using MiBlog.Api.Contracts.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        /// <returns></returns>
        //[HttpGet("{id}")]
        [Route("Get")]
        [HttpGet]
        public ActionResult Get(int id)
        {
            var comment = _commentRepository.Get(id);

            if(comment == null)
                return StatusCode(400, new { message = $"Комментарий с ID: {id} не найден." });

            var response = _mapper.Map<CommentResponse>(comment);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Получить все комментарии конкретной статьи.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{idArticle}")]
        public ActionResult GetAll(int idArticle)
        {
            var comments = _commentRepository.GetAll().Where(c => c.ArticleId == idArticle).OrderByDescending(c => c.Created);

            if (comments.Count() == 0)
                return StatusCode(400, new { message = $"К статье с ID: {idArticle} комментарии не найдены." });

            var response = _mapper.Map< Comment[], CommentResponse[]>(comments.ToArray());

            return StatusCode(200, response);
        }

        /// <summary>
        /// Создание нового комментария.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Созданные комментарий.</returns>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "User")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CommentCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Content))
                    return StatusCode(400, new { message = "Текст комментария не может быть пустым." });

                var article = _articleRepository.Get(request.IdArticle);

                if(article == null)
                    return StatusCode(400, new { message = $"Статья с ID: {request.IdArticle} не найдена." });

                var comment = new Comment();

                comment.Article = article;

                comment.User = _userRepository.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

                comment.Content = request.Content;

                comment.Created = DateTime.Now;

                _commentRepository.Create(comment);

                var response = _mapper.Map<CommentResponse>(comment);

                return StatusCode(200, response);
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующего комментария.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Сообщение о результате выполнения операции.</returns>
        [Route("Update")]
        [Authorize(Roles = "Moderator")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Update(CommentUpdateRequest request)
        {
            try
            {
                var comment = _commentRepository.Get(request.Id);

                if (comment == null)
                    return StatusCode(400, new { message = $"Комментарий с ID: {request.Id} не найден." });

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
        /// <returns>Сообщение о результате выполнения операции.</returns>
        [Authorize(Roles = "Moderator")]
        [HttpDelete("id")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var comment = _commentRepository.Get(id);

                if (comment == null)
                    return StatusCode(400, new { message = $"Комментарий с ID: {id} не найден." });

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
