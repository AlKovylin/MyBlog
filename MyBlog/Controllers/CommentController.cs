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
        public JsonResult Update(int commentId, string commentContent)
        {
            var comment = _commentRepository.Get(commentId);

            comment.Content = commentContent;

            _commentRepository.Update(comment);            

            //return RedirectToAction("Edit", "Article", model.ArticleId);//ошибка 405
            //return RedirectToAction("", "Article");
            return Json("successfully");
        }

        [Authorize(Roles = "Moderator")]
        [HttpPost]
        public JsonResult Delete(int commentId)
        {
            var comment = _commentRepository.GetAll().FirstOrDefault(c => c.Id == commentId);

            _commentRepository.Delete(comment);

            return Json("successfully");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public JsonResult Save(int articleId, string comment)
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

            return Json("successfully");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult GetAll(int articleId, string status)
        {
            var comments = _commentRepository.GetAll().Where(c => c.ArticleId == articleId).OrderByDescending(c => c.Created).ToList();

            var model = _mapper.Map<List<CommentModel>>(comments);

            ViewBag.ReadEdit = status;

            return PartialView("_CommentsPartial", model);
        }
    }
}
