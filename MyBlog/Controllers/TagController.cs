using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Infrastructure.Data.Repository;
using MyBlog.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class TagController : Controller
    {
        private readonly TagRepository _tagRepository;
        private readonly Mapper _mapper;

        public TagController(TagRepository tagRepository, Mapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create() => View("Create");

        [HttpPost]
        public IActionResult Create(string name)
        {
            var tag = new Tag() { Name = name };

            _tagRepository.Create(tag);

            return View("Create");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tags = _tagRepository.GetAll();

            var model = _mapper.Map<List<TagViewModel>>(tags);

            return View("", model);
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var tag = _tagRepository.GetAll().Where(t => t.Id == int.Parse(id)) as Tag;

            _tagRepository.Delete(tag);

            return RedirectToAction("Index");
        }
    }
}
