using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBlog.Domain.Interfaces;
using MyBlog.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        //[Authorize(Roles = "Администратор")]
        public async Task <IActionResult> Index()
        {
            //var newUser = new User()
            //{
            //    Email = "c@ya.ru",
            //    Password = "12345",
            //    Name = "Alex",
            //    DisplayName = "A",
            //    AboutMy = "Developer",
            //};

            //_userService.Create(newUser);

            //var roles = _roleService.GetList();

            //newUser.Role.Add(roles.FirstOrDefault(r => r.Name == "Admin"));

            //_userService.Update(newUser);

            //var user = _userService.Get(1);
            //var userRole = _userService.GetUserRoles(user);

            await Authenticate();

             

            //if (!_userService.Check(newUser.Email))
            //{
            //    _userService.Create(newUser);

            //    Console.WriteLine("User added");

            //    var us = _userService.GetList();

            //    foreach (var user in us)
            //    {
            //        Console.WriteLine($"{user.Name}, {user.Email}");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Пользователь с таким Email уже существует");
            //}




            //var user = _userService.Get(1);

            //_userService.Delete(user);

            //ViewData["Head"] = User.Identity.Name;
            //return View("SomeView");

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task Authenticate()//(User ClaimsPrincipal ControllerBase User{get;})
        {
            var user = _userRepository.Get(1);
            var userRoles = _userRepository.GetUserRoles(user);
          
            //var userRoles = user.Role;

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

            await HttpContext.SignInAsync(claimsPrincipal);
        }
    }
}
