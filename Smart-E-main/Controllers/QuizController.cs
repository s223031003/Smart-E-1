using Microsoft.AspNetCore.Mvc;

namespace Smart_E.Controllers
{
    public class QuizController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
