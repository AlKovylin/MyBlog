﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public UserController(IArticleRepository articleRepository,
                                 IUserRepository userRepository,
                                 IRepository<Comment> commentRepository,
                                 IRepository<Tag> tagRepository,
                                 IMapper mapper)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить данные пользователя по id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return null;
        }

        /// <summary>
        /// Получить данные всех пользователей.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{idArticle}")]
        public ActionResult GetAll(int id)
        {
            return null;
        }        

        /// <summary>
        /// Обновление данных существующего пользователя.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Update")]
        [Authorize(Roles = "Moderator")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Update()//CommentUpdateRequest request)
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Удаление пользователя по id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator")]
        [HttpDelete("id")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
