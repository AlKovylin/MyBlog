using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private IUserRepository _userRepository;
        private IRepository<Role> _roleRepository;

        public RolesController(IUserRepository userRepository, IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult Index() => View(_roleRepository.GetAll());

        [HttpGet]
        public IActionResult Create() => View();

        ///// <summary>
        ///// Создание новой роли
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult Create(string name)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var newRole = new Role() { Name = name };

        //        _roleRepository.Create(newRole);

        //        var role = _roleRepository.GetAll().FirstOrDefault(r => r.Name == name);

        //        if (role != null)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "При создании роли произошла ошибка");
        //        }
        //    }
        //    return View(name);
        //}

        ///// <summary>
        ///// Удаление роли
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpDelete]
        //public IActionResult Delete(string id)
        //{
        //    var role = _roleRepository.Get(int.Parse(id));

        //    if (role != null)
        //    {
        //        _roleRepository.Delete(role);
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public IActionResult UserList() => View(_userRepository.GetAll());

        /// <summary>
        /// Возвращает в представление данные для изменения ролей пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(string userId)
        {
            // получаем пользователя
            var user = _userRepository.Get(int.Parse(userId));

            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = _userRepository.GetUserRoles(user);
                // список всех ролей
                var allRoles = _roleRepository.GetAll();

                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id.ToString(),
                    UserEmail = user.Email,
                    UserRoles = ListNameRole(userRoles),
                    AllRoles = ListNameRole(allRoles)
                };

                return View(model);
            }
            return NotFound();
        }

        private List<string> ListNameRole(List<Role> roles)
        {
            var list = new List<string>();
            foreach (var role in roles)
            {
                list.Add(role.Name);
            }
            return list;
        }

        /// <summary>
        /// Принимает данные из представления для редактирования ролей пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            var user = _userRepository.Get(int.Parse(userId));

            if (user != null)
            {
                var allRolesInDB = _roleRepository.GetAll();// получаем все роли из бд

                user.Role.Clear();// удаляем все роли пользователя

                foreach (var role in roles)
                {                    
                    var newRole = _roleRepository.GetAll().Where(r => r.Name == role) as Role;

                    user.Role.Add(newRole);// добавляем роли в соответствии с полученным списком
                }

                _userRepository.Update(user);//обновляем данные пользователя

                return RedirectToAction("UserList");
            }
            return NotFound();
        }
    }
}
