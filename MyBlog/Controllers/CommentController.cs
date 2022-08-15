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

        public CommentController(IUserRepository userRepository, IRepository<Comment> commentRepository, IRepository<Article> articleRepository)
        {
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
        }

        [Authorize(Roles ="User")]
        [HttpGet]
        public IActionResult Create() => View("Create");

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Create(CommentViewModel model, string articleId)
        {
            if ((ModelState.IsValid))
            {
                var article = _articleRepository.Get(int.Parse(articleId));

                var user = _userRepository.GetAll().Where(u => u.Email == User.Identity.Name) as User;

                var comment = new Comment()
                {
                    Content = model.Content,
                    User = user,
                    Article = article
                };

                _commentRepository.Create(comment);

                return View();
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Create", "Некорректные данные");
            }
            
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
