using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Business.Models;
using MyBlog.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Controllers
{
    //[Authorize(Roles = "Moderator")]
    public class TagController : Controller
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public TagController(IRepository<Tag> tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }      

        [HttpGet]
        public IActionResult Index()
        {
            var tags = _tagRepository.GetAll();

            var model = new TagsViewModel() { Tags = _mapper.Map<List<TagModel>>(tags)};

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Create(string name)
        {
            var tag = new Tag() { Name = name };

            _tagRepository.Create(tag);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var tag = _tagRepository.GetAll().FirstOrDefault(t => t.Id == int.Parse(id));

            _tagRepository.Delete(tag);

            return RedirectToAction("Index");
        }
    }
}
