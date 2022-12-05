using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;

namespace Smart_E.Controllers
{
    public class QualificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QualificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Qualifications
        public async Task<IActionResult> Qualifications()
        {
            return _context.Qualifications != null ? 
                View(await _context.Qualifications.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.Qualification'  is null.");
        }

        // GET: Qualifications/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Qualifications == null)
            {
                return NotFound();
            }

            var qualification = await _context.Qualifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qualification == null)
            {
                return NotFound();
            }

            return View(qualification);
        }


    }
}
