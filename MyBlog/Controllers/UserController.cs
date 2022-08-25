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
        private readonly IArticleRepository _articleRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, 
                              IArticleRepository articleRepository, 
                              IMapper mapper,
                              IRepository<Tag> tagRepository)
        {
            _userRepository = userRepository;
            _articleRepository = articleRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }


        [Route("MyPage")]
        [HttpGet]
        public IActionResult MyPage()
        {
            var claimsPrincipal = User;

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == claimsPrincipal.Identity.Name);

            var articles = _articleRepository.GetAll().Select(a => a).Where(a => a.User == user).ToList();

            var model = new ArticlesAllViewModel<ArticleUserViewModel>();

            if (articles != null)
            {
                foreach (var article in articles)
                {
                    model.AllArticles.Add(new ArticleUserViewModel()
                    {
                        Article = _mapper.Map<ArticleModel>(article),
                        Tags = _mapper.Map<List<TagModel>>(_articleRepository.GetArticleTags(article))
                    });
                }
            }

            return View("MyPage", model);
        }
            
        [Route("Edit")]
        [HttpGet]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Edit()
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            var editmodel = _mapper.Map<UserViewModel>(user);

            //editmodel.FirstName = user.

            return View("UserData", editmodel);
        }

        //[Route("Udate")]
        //[HttpPost]
        //[Authorize(Roles = "User, Moderator")]
        //public IActionResult Update(UserEditViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = _userRepository.Get(int.Parse(model.Id));

        //        user.Email = model.Email;
        //        user.Name = model.Name;
        //        user.DisplayName = model.DisplayName;
        //        user.AboutMy = model.AboutMy;
        //        user.Photo = model.Photo;

        //        _userRepository.Update(user);

        //        return RedirectToAction("MyPage", "User");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Некорректные данные");
        //        return View("Edit", model);
        //    }
        //}

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
