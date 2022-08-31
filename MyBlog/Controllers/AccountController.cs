﻿using Microsoft.AspNetCore.Mvc;
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

            return View("LoginError");
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

                    return RedirectToAction("Edit", "User");
                }
            }
            ViewData["checkEmail"] = email;

            return View("/Views/Account/CheckUser.cshtml");
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Получение списка всех пользователей
        /// </summary>
        /// <returns></returns>
        [Route("GetUsers")]
        [HttpGet]
        [Authorize(Roles = "Moderator, Admin")]
        public IActionResult GetUsers()
        {
            var model = new UsersAllViewModel();

            var users = _userRepository.GetAll();

            foreach (var user in users)
            {
                var roles = _userRepository.GetUserRoles(user);

                var userModel = _mapper.Map<UserViewModel>(user);

                userModel.Roles = _mapper.Map<List<RoleModel>>(roles);

                model.Users.Add(userModel);
            }

            return View(model);
        }

        /// <summary>
        /// Редактиование пользователя для Moderator, Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        [Authorize(Roles = "Moderator, Admin")]
        public IActionResult Edit(int id)
        {
            var user = _userRepository.Get(id);

            //скользкое решение, если имя или фамилия будут состоять из двух слов
            var editmodel = _mapper.Map<UserViewModel>(user);

            var userRoles = _userRepository.GetUserRoles(user);

            editmodel.Roles = _mapper.Map<List<RoleModel>>(userRoles);

            var allRoles = _roleRepository.GetAll();

            editmodel.AllRoles = _mapper.Map<List<RoleModel>>(allRoles);

            return View("UserData", editmodel);
        }

        /// <summary>
        /// Сохраняет в базу данные пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="AboutMy"></param>
        /// <param name="RolesList"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Moderator, Admin")]
        public IActionResult Save(UserViewModel model, string AboutMy, List<string> RolesList)
        {
            var user = _userRepository.Get(model.id);

            //user = _mapper.Map<User>(model);//происходт ошибка записи в бд

            user.AboutMy = AboutMy;

            user.Name = model.FirstName + " " + model.LastName;

            user.Email = model.Email;

            user.DisplayName = model.DisplayName;

            user.AboutMy = AboutMy;

            var userRoles = _userRepository.GetUserRoles(user);

            foreach (var role in userRoles)
            {
                if (role.Name != "User")
                    user.Role.Remove(role);
            }

            foreach (var role in RolesList)
            {
                var _role = _roleRepository.GetAll().FirstOrDefault(r => r.Name == role);
                user.Role.Add(_role);
            }

            _userRepository.Update(user);

            return RedirectToAction("GetUsers", "Account");
        }

        //ОСТАВИТЬ ТОЛЬКО МОДЕРАТОРА или админа
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
