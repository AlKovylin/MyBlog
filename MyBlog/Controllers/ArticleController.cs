﻿using AutoMapper;
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

            var articles = _articleRepository.GetAll();

            if (articles != null)
            {

                foreach (var article in articles)
                {
                    var user = _userRepository.GetAll().FirstOrDefault(u => u.Id == article.UserId);

                    var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).ToList();

                    var tags = _articleRepository.GetArticleTags(article);

                    allArticles.AllArticles.Add(new ArticleViewModel
                    {
                        Article = _mapper.Map<ArticleModel>(article),
                        Author = _mapper.Map<UserModel>(user),
                        Comments = _mapper.Map<List<CommentModel>>(comments),
                        Tags = _mapper.Map<List<TagModel>>(tags)
                    });
                }

                return View("Index", allArticles);
            }
            else
            {
                return View("Articles", "Пока здесь нет ни одной статьи.");
            }
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

            var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).ToList();

            var tags = _articleRepository.GetArticleTags(article);

            var model = new ArticleViewModel()
            {
                Article = _mapper.Map<ArticleModel>(article),
                Author= _mapper.Map<UserModel>(user),
                Comments= _mapper.Map<List<CommentModel>>(comments),
                Tags= _mapper.Map<List<TagModel>>(tags)
            };

            return View("ReadArticle", model);
        }

        /// <summary>
        /// Получение своих статей авторизованным пользователем
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult AutorArticles() 
        {
            var author = _userRepository.GetAll().Where(u => u.Email == User.Identity.Name) as User;

            var articles = _articleRepository.GetAll().Where(a => a.User == author);

            if (articles != null)
            {
                return View("AuthorArticles", articles);
            }
            else
            {
                return View("AuthorArticles", "У вас пока нет статей.");
            }
        }

        /// <summary>
        /// Получение конкретной статьи для редактирования
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User, Moderator")]
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

            return View("Editor", editArticle);
        }

        /// <summary>
        /// Сохранение отредактированной статьи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Save(ArticleEditViewModel model, string content, List<string> TagsList)
        {
            if (model.Article.Id > 0)
            {
                var article = _articleRepository.Get(model.Article.Id);

                article.Title = model.Article.Title;

                article.Content = content;

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

                newArticle.Content = content;

                newArticle.User = _userRepository.GetAll().Select(u => u).FirstOrDefault(u => u.Email == User.Identity.Name);

                foreach (var tag in TagsList)
                {
                    var _tag = _tagRepository.GetAll().FirstOrDefault(t => t.Name == tag);
                    newArticle.Tags.Add(_tag);
                }

                newArticle.Published = DateTime.Now;

                _articleRepository.Create(newArticle);
            }
            return RedirectToAction("MyPage", "User");
            //return RedirectToAction("Edit", "Article", new { id = article.Id });
            //return View("Editor", article.Id);
            //return RedirectToAction("Index", "Article");
            //return RedirectToActionPermanent("Edit", new { id = article.Id });
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
        public IActionResult Create(ArticleCreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var newArticle = _mapper.Map<Article>(model);

                foreach(var tag in model.Tags)
                {
                    newArticle.Tags.Add(_tagRepository.Get(tag.Id));
                }

                newArticle.User = _userRepository.GetAll().Where(u => u.Email == User.Identity.Name) as User;

                _articleRepository.Create(newArticle);

                return RedirectToAction("MyPage", "User");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Create", model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Delete(string id)
        {
            var article = _articleRepository.Get(int.Parse(id));

            _articleRepository.Delete(article);

            return View();
        }
    }
}
