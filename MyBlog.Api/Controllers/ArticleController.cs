using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public ArticleController(IArticleRepository articleRepository,
                                 IRepository<Comment> commentRepository,
                                 IRepository<Tag> tagRepository,
                                 IUserRepository userRepository,
                                 IMapper mapper)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (User.Identity.IsAuthenticated)
            {
                var articles = _articleRepository.GetAll();

                return StatusCode(200, articles);
            }
            return StatusCode(403, new { message = "Недостаточно прав для доступа к ресурсу!" });
        }

        /// <summary>
        /// Чтение конкретной статьи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetArticle")]
        [HttpPost]
        public IActionResult Get(int id)
        {
            var article = _articleRepository.GetAll().FirstOrDefault(a => a.Id == id);

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Id == article.UserId);

            var comments = _commentRepository.GetAll().Select(c => c).Where(c => c.ArticleId == article.Id).OrderByDescending(c => c.Created).ToList();

            var tags = _articleRepository.GetArticleTags(article);

            var model = new ArticleViewModel()
            {
                Article = _mapper.Map<ArticleModel>(article),
                Author = _mapper.Map<UserModel>(user),
                Comments = _mapper.Map<List<CommentModel>>(comments),
                TagsArticle = _mapper.Map<List<TagModel>>(tags)
            };

            ViewBag.ReadEdit = "Read";

            return View("ReadArticle", model);
        }
    }
}
