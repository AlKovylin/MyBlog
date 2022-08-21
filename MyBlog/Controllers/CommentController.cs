using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
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

        public CommentController(IUserRepository userRepository, IRepository<Comment> commentRepository, IArticleRepository articleRepository)
        {
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
        }

        //[Authorize(Roles ="User")]
        //[HttpGet]
        //public IActionResult Create() => View("Create");

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

            ViewData["ArticleId"] = articleId;

            return RedirectToAction("Read", "Article");

        }

        [Authorize(Roles = "User, Moderator")]
        [HttpDelete]
        public IActionResult Delete(string commentId)
        {
            var comment = _commentRepository.GetAll().Where(c => c.Id == int.Parse(commentId)) as Comment;

            _commentRepository.Delete(comment);

            return View();
        }

    }
}
