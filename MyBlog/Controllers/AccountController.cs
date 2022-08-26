using Microsoft.AspNetCore.Mvc;
using MyBlog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MyBlog.Domain.Core;
using System.Linq;
using System;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using MyBlog.ViewModels;
using AutoMapper;
using MyBlog.Infrastructure.Business.Models;

namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;
        private IRepository<Role> _roleRepository;
        private IMapper _mapper;

        public AccountController(IUserRepository userRepository, IRepository<Role> roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Принимает данные из представления и обеспечивает вход в систему
        /// </summary>    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password)
        {

            // поиск пользователя в бд
            User user = null;

            user = _userRepository.GetAll().FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                await Authenticate(user);
                return RedirectToAction("Index", "Article");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register() => View("Register");


        /// <summary>
        /// Принимает данные из представления и обеспечивает регистрацию в системе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password)//RegisterViewModel
        {
            if (ModelState.IsValid)
            {
                User user = null;

                user = _userRepository.GetAll().FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    // добавляем пользователя в бд
                    var newUser = new User();

                    newUser.Email = email;
                    newUser.Password = password;
                    newUser.Role.Add(_roleRepository.GetAll().FirstOrDefault(r => r.Name == DefaultRole.Role));

                    _userRepository.Create(newUser);

                    //проверяем успешность добавления в базу
                    user = _userRepository.GetAll().Where(u => u.Email == email && u.Password == password).FirstOrDefault();

                    var model = _mapper.Map<UserViewModel>(user);

                    if (user != null)
                    {
                        await Authenticate(user);

                        return View("UserData", model);
                    }
                }
                else
                {
                    //Здесь добавить возврат ошибки
                    Console.WriteLine("Пользователь с таким логином уже существует");
                }
            }
            return RedirectToAction("Index", "Article");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Article");
        }

        /// <summary>
        /// Регистрирует пользователя в системе
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task Authenticate(User user)//(User ClaimsPrincipal ControllerBase User{get;})
        {
            var userRoles = _userRepository.GetUserRoles(user);

            //создаем claim на основе e-mail
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
            };

            //добавляем клаймы на основе ролей пользователя
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "AppCookie");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);//устанавливаем куки для пользователя
        }

        //ОСТАВИТЬ ТОЛЬКО МОДЕРАТОРА
        [HttpPost]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult Save(UserViewModel model, string AboutMy)
        {
            var user = _userRepository.Get(model.id);

            user.AboutMy = AboutMy;

            user.Name = model.FirstName + " " + model.LastName;

            user.Email = model.Email;

            user.DisplayName = model.DisplayName;

            user.AboutMy = AboutMy;

            _userRepository.Update(user);

            return RedirectToAction("GetUsers");
        }

        //ОСТАВИТЬ ТОЛЬКО МОДЕРАТОРА
        [HttpGet]
        [Authorize(Roles = "User, Moderator")]
        public IActionResult GetUsers()
        {
            var model = new UsersAllViewModel();

            var users = _userRepository.GetAll();

            foreach(var user in users)
            {
                var roles = _userRepository.GetUserRoles(user);

                var userModel = _mapper.Map<UserViewModel>(user);

                userModel.Roles = _mapper.Map<List<RoleModel>>(roles);

                model.Users.Add(userModel);
            }

            return View(model);
        }

        //ОСТАВИТЬ ТОЛЬКО МОДЕРАТОРА или админа
        [HttpDelete]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Delete(int id)
        {
            var user = _userRepository.Get(id);

            if (user != null)
            {
                _userRepository.Delete(user);
            }
            return RedirectToAction("GetUsers");
        }

    }
}
