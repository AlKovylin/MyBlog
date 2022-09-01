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
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private IUserRepository _userRepository;
        private IRepository<Role> _roleRepository;
        private IMapper _mapper;

        public RoleController(IUserRepository userRepository, IRepository<Role> roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleRepository.GetAll();

            var model = new RolesViewModel()
            {
                AllRoles = _mapper.Map<List<RoleModel>>(roles)
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Edit(int id)
        {
            var role = _roleRepository.Get(id);

            var model = _mapper.Map<RoleViewModel>(role);

            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult Save(RoleViewModel model, string description)
        {
            var role = _roleRepository.Get(model.Id);

            if (role == null)
            {
                var newRole = new Role()
                {
                    Name = model.Name,
                    Description = description
                };
                _roleRepository.Create(newRole);

                return RedirectToAction("Index");
            }

            role.Description = description;

            _roleRepository.Update(role);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var role = _roleRepository.GetAll().FirstOrDefault(r => r.Id == id);

            _roleRepository.Delete(role);

            return RedirectToAction("Index");
        }
    }
}
