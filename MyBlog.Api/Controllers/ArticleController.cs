using AutoMapper;
using MiBlog.Api.Contracts.Models.Articles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Business.Models;
using System;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;//не используется в коде, но добавляет автора статьи 
        private readonly IRepository<Comment> _commentRepository;//не используется в коде, но добавляет комментарии к статье
        private readonly IRepository<Tag> _tagRepository;//с IRepository<Tag> это не работает, видимо это связано со связью многие ко многим
        private readonly IMapper _mapper;

        public ArticleController(IArticleRepository articleRepository,
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
        /// Получение всех статей.
        /// </summary>
        /// <returns>Статьи, комментарии к ним и теги, сведения об авторе.</returns>
        [Route("GetAll")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var articles = _articleRepository.GetAll().OrderByDescending(a => a.Published);

            var response = new ArticlesResponse();

            foreach (var article in articles)
            {
                article.Tags = _articleRepository.GetArticleTags(article);

                response.Articles.Add(CreateModel(article));
            }

            return StatusCode(200, response.Articles.ToArray());
        }

        /// <summary>
        /// Получение конкретной статьи по id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Статья, комментарии к ней и теги, сведения об авторе.</returns>
        //[Route("Get")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var article = _articleRepository.Get(id);

            if (article is null)
                return StatusCode(400, new { message = $"Сатья с ID: {id} не найдена." });

            article.Tags = _articleRepository.GetArticleTags(article);

            var response = CreateModel(article);

            return StatusCode(200, response);
        }

        /// <summary>
        /// Маппинг ArticleResponse.
        /// </summary>
        /// <param name="article"></param>
        /// <returns>new ArticleResponse.</returns>
        private ArticleResponse CreateModel(Article article)
        {
            return new ArticleResponse()
            {
                Article = _mapper.Map<ArticleModel>(article),
                Author = _mapper.Map<UserModel>(article.User),
                Comments = _mapper.Map<Comment[], CommentModel[]>(article.Comment.ToArray()),
                TagsArticle = _mapper.Map<Tag[], TagModel[]>(article.Tags.ToArray())
            };
        }

        /// <summary>
        /// Создание новой статьи.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Сообщение о результате выполнения функции.</returns>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Create(ArticleRequest request)
        {
            try
            {
                var article = new Article();

                article.Title = request.Article.Title;

                article.Content = request.Article.Content;

                foreach (var tag in request.TagsArticle)
                {
                    var _tag = _tagRepository.Get(tag.Id);
                    article.Tags.Add(_tag);
                }

                article.User = _userRepository.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

                article.Published = DateTime.Now;

                _articleRepository.Create(article);

                return StatusCode(200, "Создание статьи прошло успешно.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующей статьи.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Сообщение о результате выполнения функции.</returns>
        [Route("Update")]
        [HttpPut]
        [Authorize(Roles = "User")]
        public IActionResult Update(ArticleRequest request)
        {
            try
            {
                var article = _articleRepository.Get(request.Article.Id);

                article.Title = request.Article.Title;

                article.Content = request.Article.Content;

                article.Modified = DateTime.Now;

                article.Tags = _articleRepository.GetArticleTags(article);

                article.Tags.Clear();

                foreach (var tag in request.TagsArticle)
                {
                    var _tag = _tagRepository.Get(tag.Id);
                    article.Tags.Add(_tag);
                }

                _articleRepository.Update(article);

                return StatusCode(200, "Обновление прошло успешно.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Удаление статьи.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Сообщение о результате выполнения функции.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult Delete(int id)
        {
            try
            {
                var article = _articleRepository.Get(id);

                if (article is null)
                    return StatusCode(400, new { message = $"Сатья с ID: {id} не найдена." });

                _articleRepository.Delete(article);

                return StatusCode(200, "Удаление прошло успешно.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }
    }
}
