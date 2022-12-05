using Microsoft.AspNetCore.Mvc;
using Smart_E.Data;
using Smart_E.Models.Courses;

namespace Smart_E.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Course([FromQuery] Guid? id)
        {
           
            
            ChapterViewModel chapterViewModel = new ChapterViewModel();
          
            chapterViewModel.chapters = _context.Chapter.OrderBy(c=> c.ChapterName).ToList();
           
            return View(chapterViewModel);
        }
    }
}
