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


namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;
        private IRepository<Role> _roleRepository;

        public AccountController(IUserRepository userRepository, IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
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

        /// <summary>
        /// Принимает данные из представления и обеспечивает регистрацию в системе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;

                user = _userRepository.GetAll().FirstOrDefault(u => u.Email == model.Email);

                if (user == null)
                {
                    // добавляем пользователя в бд
                    var newUser = new User();

                    newUser.Email = model.Email;
                    newUser.Password = model.Password;
                    newUser.Role.Add(_roleRepository.GetAll().FirstOrDefault(r => r.Name == DefaultRole.Role));

                    _userRepository.Create(newUser);

                    //проверяем успешность добавления в базу
                    user = _userRepository.GetAll().Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();

                    if (user != null)
                    {
                        await Authenticate(user);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    Console.WriteLine("Пользователь с таким логином уже существует");
                }
            }
            return View(model);
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
    }
}
