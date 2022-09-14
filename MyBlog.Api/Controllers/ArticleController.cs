using AutoMapper;
using MiBlog.Api.Contracts.Models.Articles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;//не используется в коде, но добавляет автора статьи 
        private readonly IRepository<Comment> _commentRepository;//не используется в коде, но добавляет комментарии к статье
        //с IRepository<Tag> это не работает, видимо это связано со связью многие ко многим
        private readonly IMapper _mapper;

        public ArticleController(IArticleRepository articleRepository,
                                 IUserRepository userRepository,
                                 IRepository<Comment> commentRepository,
                                 IMapper mapper)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

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
        /// Чтение конкретной статьи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Get")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            var article = _articleRepository.Get(id);

            if (article is null)
                return StatusCode(400, new { message = $"Сатья с ID: {id} не найдена." });

            article.Tags = _articleRepository.GetArticleTags(article);

            var response = CreateModel(article);

            return StatusCode(200, response);
        }

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

        [Route("Update")]
        [HttpPut]
        public IActionResult Update(ArticleRequest request)
        {
            try
            {
                var article = _articleRepository.Get(request.Article.Id);

                article.Title = request.Article.Title;
                article.Content = request.Article.Content;

                article.Tags.Clear();
                article.Tags = _mapper.Map<List<Tag>>(request.TagsArticle);

                _articleRepository.Update(article);

                return StatusCode(200, "Обновление прошло успешно.");
            }
            catch
            {
                return StatusCode(500, "Что-то пошло не так.");
            }
        }

        [Route("Delete")]
        [HttpDelete]
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
