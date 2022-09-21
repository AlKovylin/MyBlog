using AutoMapper;
using MiBlog.Api.Contracts.Models.Articles;
using MiBlog.Api.Contracts.Models.Info;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Business.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;//не используется в коде формирования статьи, но добавляет автора статьи 
        private readonly IRepository<Comment> _commentRepository;//не используется в коде формирования статьи, но добавляет комментарии к статье
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
        /// <response code="200">Массив [статья, комментарии + автор комментария, теги статьи, сведения об авторе статьи]. Модель <a href='#model-ArticlesResponse'>ArticlesResponse</a></response>
        [Route("GetAll")]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ArticlesResponse[]))]
        public IActionResult GetAll()
        {
            var articles = _articleRepository.GetAll().OrderByDescending(a => a.Published);

            var list = new List<ArticleResponse>();

            foreach (var article in articles)
            {
                article.Tags = _articleRepository.GetArticleTags(article);

                //article.Comment = _commentRepository.GetAll().Where(c => c.Article == article).ToList();//работает без этого, почему?

                list.Add(CreateModel(article));
            }

            var response = new ArticlesResponse();

            response.Articles = list.ToArray();

            return StatusCode(200, response.Articles);

        }

        /// <summary>
        /// Получение конкретной статьи по id.
        /// </summary>
        /// <param name="id">ID статьи.</param>
        /// <response code="200">Статья, комментарии + автор комментария, теги статьи, сведения об авторе статьи. Модель <a href='#model-ArticlesResponse'>ArticlesResponse</a></response>
        /// <response code="204">"Сатья с ID: {id} не найдена."</response>
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ArticleResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        public IActionResult Get(int id)
        {
            var article = _articleRepository.Get(id);

            if (article is null)
                return StatusCode(204, new { message = $"Сатья с ID: {id} не найдена." });

            article.Tags = _articleRepository.GetArticleTags(article);

            //article.Comment = _commentRepository.GetAll().Where(c => c.Article == article).ToList();//работает без этого, почему?

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
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///       "article": {     
        ///         "title": "Заголовок статьи.",
        ///         "content": "Текст статьи."
        ///       },
        ///       "tagsArticle": [
        ///       {
        ///         "id": 2,
        ///         "name": "#EF"
        ///       },
        ///       {
        ///         "id": 3,
        ///         "name": "#C#"
        ///       }
        ///      ]
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-ArticleRequest'>ArticleRequest</a></param>
        /// <response code="201">Создание статьи прошло успешно.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status201Created)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
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

                return StatusCode(201, "Создание статьи прошло успешно.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        /// <summary>
        /// Обновление существующей статьи.
        /// </summary>
        /// <remarks>
        /// Образец запроса:
        ///
        ///     POST
        ///     {
        ///       "article": {
        ///         "id": 1,
        ///         "title": "Заголовок статьи.",
        ///         "content": "Текст статьи."
        ///       },
        ///       "tagsArticle": [
        ///       {
        ///         "id": 2,
        ///         "name": "#EF"
        ///       },
        ///       {
        ///         "id": 3,
        ///         "name": "#C#"
        ///       }
        ///      ]
        ///     }
        /// </remarks>
        /// <param name="request">Модель <a href='#model-ArticleRequest'>ArticleRequest</a></param>
        /// <response code="200">Обновление прошло успешно.</response>
        /// <response code="204">Статья с указанным ID не найдена.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [Route("Update")]
        [HttpPut]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public IActionResult Update(ArticleRequest request)
        {
            try
            {
                var article = _articleRepository.Get(request.Article.Id);

                if (article is null)
                    return StatusCode(204, new { message = $"Сатья с ID: {request.Article.Id} не найдена." });

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
        /// <param name="id">ID статьи.</param>
        /// <response code="200">Удаление статьи прошло успешно.</response>
        /// <response code="204">Статья с указанным ID не найдена.</response>
        /// <response code="401">Unauthorized: доступно пользователям Roles="User".</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status204NoContent, null, typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
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
