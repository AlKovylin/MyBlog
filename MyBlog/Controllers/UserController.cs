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
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, 
                              IArticleRepository articleRepository, 
                              IMapper mapper,
                              IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _articleRepository = articleRepository;
            _roleRepository = roleRepository;
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
        [Authorize(Roles = "User")]
        public IActionResult Edit()
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            var editmodel = _mapper.Map<UserViewModel>(user);

            return View("UserData", editmodel);
        }     

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Index() => View(_userRepository.GetAll());




        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Save(UserViewModel model, string AboutMy)
        {
            var user = _userRepository.Get(model.id);

            //user = _mapper.Map<User>(model);

            user.AboutMy = AboutMy;

            user.Name = model.FirstName + " " + model.LastName;

            user.Email = model.Email;

            user.DisplayName = model.DisplayName;

            user.AboutMy = AboutMy;                        

            _userRepository.Update(user);

            return RedirectToAction("Mypage", "User");
        }
    }
}
