using AutoMapper;
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
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IRepository<Article> articleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _articleRepository = articleRepository;
            _mapper = mapper;
        }


        [Route("MyPage")]
        [HttpGet]
        public IActionResult MyPage()
        {
            var claimsPrincipal = User;

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == claimsPrincipal.Identity.Name);

            var articles = _articleRepository.GetAll().Select(a => a).Where(a => a.User == user).ToList();

            var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).ToList();

            var model = new UserViewModel()
            {
                User = _mapper.Map<UserModel>(user),
                Articles = _mapper.Map<List<ArticleModel>>(articles),

            };

            return View("User", model);
        }
            
        [Route("Edit")]
        [HttpGet]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Edit()
        {
            var user = _userRepository.GetAll().Where(u => u.Email == User.Identity.Name);

            var editmodel = _mapper.Map<UserEditViewModel>(user);

            return View("Edit", editmodel);
        }

        [Route("Udate")]
        [HttpPost]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.Get(int.Parse(model.Id));

                user.Email = model.Email;
                user.Name = model.Name;
                user.DisplayName = model.DisplayName;
                user.AboutMy = model.AboutMy;
                user.Photo = model.Photo;

                _userRepository.Update(user);

                return RedirectToAction("MyPage", "User");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Index() => View(_userRepository.GetAll());


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            var user = _userRepository.Get(int.Parse(id));

            if (user != null)
            {
                _userRepository.Delete(user);
            }
            return RedirectToAction("Index");
        }
    }
}
