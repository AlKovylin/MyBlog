﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Business.Models;
using MyBlog.ViewModels;
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
            ViewData["Authorise"] = HttpContext.User.Identity.IsAuthenticated;

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

        //[Route("Read")]
        //[HttpPost]
        //public IActionResult Read(ArticleViewModel model)
        //{
        //    return View("ReadArticle", model);

        //    //return RedirectToPage("ReadArticle", model);
        //}

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
        [HttpGet]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Edit(string articleId)
        {
            var article = _articleRepository.Get(int.Parse(articleId));

            var getArticle = _mapper.Map<ArticleEditViewModel>(article);

            var tags = _articleRepository.GetArticleTags(article);

            getArticle.Tags = _mapper.Map<List<TagModel>>(tags);

            return View("Edit", getArticle);
        }

        /// <summary>
        /// Сохранение отредактированной статьи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Edit(ArticleEditViewModel model)
        {
            var article = _articleRepository.Get(int.Parse(model.Id));

            article.Title = model.Title;
            article.Content = model.Content;

            article.Tags.Clear();

            foreach(var tag in model.Tags)
            {
                var newTag = _tagRepository.GetAll().Where(t => t.Name == tag.Name) as Tag;

                article.Tags.Add(newTag);
            }

            return View("Edit");
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            var tags = _tagRepository.GetAll();

            return View("Create", tags);
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