using AutoMapper;
using MiBlog.Api.Contracts.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public CommentController(IArticleRepository articleRepository,
                                 IUserRepository userRepository,
                                 IRepository<Comment> commentRepository,
                                 IRepository<Tag> tagRepository,
                                 IMapper mapper)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить комментарий по id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return null;
        }

        /// <summary>
        /// Получить все комментарии конкретной статьи.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{idArticle}")]
        public ActionResult GetAll(int id)
        {
            return null;
        }

        /// <summary>
        /// Создание нового комментария.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "User")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CommentCreateRequest request)
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Обновление существующего комментария.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Update")]
        [Authorize(Roles = "Moderator")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Update(CommentUpdateRequest request)
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Удаление комментария по id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator")]
        [HttpDelete("id")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
