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
    [Authorize(Roles = "Moderator")]
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
        public IActionResult Edit(int id)
        {
            var model = new TagViewModel();

            if (id > 0)
            {
                var tag = _tagRepository.Get(id);

                model = _mapper.Map<TagViewModel>(tag);
            }

            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult UpdateDelete(TagViewModel model, string action)
        {
            var tag = _tagRepository.Get(model.Id);

            if (action == "update")
            {                
                tag.Name = model.Name;

                _tagRepository.Update(tag);
            }
            else if (action == "delete")
            {
                _tagRepository.Delete(tag);
            }

            return RedirectToAction("Index");
        }
    }
}
