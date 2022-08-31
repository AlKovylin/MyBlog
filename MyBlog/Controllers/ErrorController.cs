using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Errors(string id)
        {
            switch (id)
            {
                case "401":
                case "403":
                    return View($"~/Views/Shared/Error.cshtml", ViewData["Error"] = "У вас не достаточно прав.");
                case "404":
                    return View($"~/Views/Shared/Error.cshtml", ViewData["Error"] = "Ресурс не найден или у вас недостаточно прав.");
                default: 
                    return View($"~/Views/Shared/Error.cshtml", ViewData["Error"] = "Что-то пошло не так.");
            }
        }
    }
}
