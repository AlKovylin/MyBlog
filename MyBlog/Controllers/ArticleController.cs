using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Business.Models;
using MyBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ArticleController(IArticleRepository articleRepository,
                                 IRepository<Comment> commentRepository,
                                 IRepository<Tag> tagRepository,
                                 IUserRepository userRepository,
                                 IMapper mapper)
        {
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все статьи
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var allArticles = new ArticlesAllViewModel<ArticleViewModel>();

            var articles = _articleRepository.GetAll().OrderByDescending(a => a.Published).ToList();

            //получаем все теги
            var allTags = _tagRepository.GetAll();
            //мапим в список TagModels
            var allTagsModel = _mapper.Map<List<TagModel>>(allTags);
            //заполняем поле содержащее количество статей по тегу
            foreach (var tag in allTagsModel)
            {
                tag.NumArticlesByTag = _articleRepository.GetNumArticlesByTag(allTags.FirstOrDefault(t => t.Id == tag.Id));
            }
            //записываем в модель
            allArticles.AllTags = allTagsModel;

            //формируем данные для отдельных статей
            foreach (var article in articles)
            {
                var user = _userRepository.GetAll().FirstOrDefault(u => u.Id == article.UserId);

                var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).ToList();

                var commentsModelList = new List<CommentModel>();

                foreach (var comment in comments)
                {
                    var _user = _userRepository.GetAll().FirstOrDefault(u => u.Id == comment.UserId);

                    var _comment = _mapper.Map<CommentModel>(comment);

                    _comment.User = _mapper.Map<UserModel>(_user);

                    commentsModelList.Add(_comment);
                }

                var tags = _articleRepository.GetArticleTags(article);

                allArticles.AllArticles.Add(new ArticleViewModel
                {
                    Article = _mapper.Map<ArticleModel>(article),
                    Author = _mapper.Map<UserModel>(user),
                    Comments = commentsModelList,
                    TagsArticle = _mapper.Map<List<TagModel>>(tags)
                });
            }

            return View("Index", allArticles);
        }

        /// <summary>
        /// Чтение конкретной статьи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpPost]
        public IActionResult Read(int id)
        {
            var article = _articleRepository.GetAll().FirstOrDefault(a => a.Id == id);

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Id == article.UserId);

            var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).OrderByDescending(c => c.Created).ToList();

            var tags = _articleRepository.GetArticleTags(article);

            var model = new ArticleViewModel()
            {
                Article = _mapper.Map<ArticleModel>(article),
                Author = _mapper.Map<UserModel>(user),
                Comments = _mapper.Map<List<CommentModel>>(comments),
                TagsArticle = _mapper.Map<List<TagModel>>(tags)
            };

            ViewBag.ReadEdit = "Read";

            return View("ReadArticle", model);
        }

        /// <summary>
        /// Получение конкретной статьи для редактирования
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Edit(int Id)
        {
            var article = _articleRepository.Get(Id);

            var tagsArticle = _articleRepository.GetArticleTags(article);

            var tagsAll = _tagRepository.GetAll();

            var editArticle = new ArticleEditViewModel()
            {
                Article = _mapper.Map<ArticleModel>(article),
                TagsArticle = _mapper.Map<List<TagModel>>(tagsArticle),
                TagsAll = _mapper.Map<List<TagModel>>(tagsAll),
            };

            if (User.IsInRole("Moderator"))
            {
                var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).OrderByDescending(c => c.Created).ToList();

                editArticle.Comments = _mapper.Map<List<CommentModel>>(comments);
            }

            ViewBag.ReadEdit = "Edit";

            return View("Editor", editArticle);
        }

        /// <summary>
        /// Сохранение отредактированной или новой статьи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateInput(false)]
        [Authorize(Roles = "User")]
        public JsonResult Save(ArticleEditViewModel model, List<string> TagsList)
        {
            if (model.Article.Id > 0)
            {
                var article = _articleRepository.Get(model.Article.Id);

                article.Title = model.Article.Title;

                article.Content = model.Article.Content;

                var articleTags = _articleRepository.GetArticleTags(article);

                foreach (var tag in articleTags)
                {
                    article.Tags.Remove(tag);
                }

                foreach (var tag in TagsList)
                {
                    var _tag = _tagRepository.GetAll().FirstOrDefault(t => t.Name == tag);
                    article.Tags.Add(_tag);
                }

                article.Modified = DateTime.Now;

                _articleRepository.Update(article);
            }
            else//если нет создадим
            {
                var newArticle = _mapper.Map<Article>(model.Article);

                newArticle.Content = model.Article.Content;

                newArticle.User = _userRepository.GetAll().Select(u => u).FirstOrDefault(u => u.Email == User.Identity.Name);

                foreach (var tag in TagsList)
                {
                    var _tag = _tagRepository.GetAll().FirstOrDefault(t => t.Name == tag);
                    newArticle.Tags.Add(_tag);
                }

                newArticle.Published = DateTime.Now;

                _articleRepository.Create(newArticle);
            }

            return Json("Сохранение статьи прошло успешно!");
        }


        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            var tagsAll = _tagRepository.GetAll();

            var article = new ArticleEditViewModel()
            {
                Article = new ArticleModel(),
                TagsAll = _mapper.Map<List<TagModel>>(tagsAll),
            };

            article.Article.Title = "";

            article.Article.Content = "";

            return View("Editor", article);
        }



        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Delete(string id)
        {
            var article = _articleRepository.Get(int.Parse(id));

            _articleRepository.Delete(article);

            return RedirectToAction("MyPage", "User");
        }
    }
}
