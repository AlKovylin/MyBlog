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
using System.Threading.Tasks;

namespace MyBlog.Controllers
{

    public class CommentController : Controller
    {
        private readonly IUserRepository _userRepository;
        private IRepository<Comment> _commentRepository;
        private IRepository<Article> _articleRepository;
        private IMapper _mapper;

        public CommentController(IUserRepository userRepository, IRepository<Comment> commentRepository, IArticleRepository articleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Create(string comment, int articleId)
        {

            var article = _articleRepository.Get(articleId);

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            var newComment = new Comment()
            {
                Content = comment,
                User = user,
                Article = article
            };

            _commentRepository.Create(newComment);

            //return RedirectToAction("Read", "Article", new { id = articleId});//ошибка 405
            return RedirectToAction("Index", "Article");
        }

        [Authorize(Roles = "Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, int articleId)
        {
            var comment = _commentRepository.Get(id);

            var model = _mapper.Map<CommentViewModel>(comment);

            model.ArticleId = articleId;

            return View(model);
        }

        [Authorize(Roles = "Moderator")]
        [HttpPost]
        public IActionResult Update(CommentViewModel model)
        {
            var comment = _commentRepository.Get(model.Id);

            comment.Content = model.Content;

            _commentRepository.Update(comment);

            //return RedirectToAction("Edit", "Article", model.ArticleId);//ошибка 405
            return RedirectToAction("", "Article");
        }

        [Authorize(Roles = "Moderator")]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            var comment = _commentRepository.GetAll().FirstOrDefault(c => c.Id == Id);

            _commentRepository.Delete(comment);

            return RedirectToAction("Index", "Article");
        }
    }
}
