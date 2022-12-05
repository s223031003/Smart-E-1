using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using System.Diagnostics.Metrics;

namespace Smart_E.Controllers
{
    public class HODController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HODController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Grade()
        {
            IEnumerable<Grade> objList = _db.Grades;
            return View(objList);
        }
        public IActionResult ViewSubjects()
        {
            IEnumerable<Subject> objList = _db.Subjects;
            return View(objList);
        }
        public IActionResult CreateSubject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSubject(Subject obj)
        {
            if (ModelState.IsValid)
            {
                _db.Subjects.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Grade");
            }
            return View(obj);
        }
        public IActionResult GetTeachers()
        {
            return View();

        }
        public IActionResult Assign()
        {
            //var teachers = await(
            //   from u in _db.Users
            //   join ur in _db.UserRoles
            //       on u.Id equals ur.UserId
            //   join r in _db.Roles
            //    on ur.RoleId equals r.Id
            //   where r.Name == "Teacher"
            //   select new
            //   {
            //       Id = u.Id,
            //       Name = u.FirstName + " " + u.LastName,                 
            //   }).ToListAsync();
            //ViewBag.message = teachers;
            //return Json(teachers);
            List<Subject> cl = new List<Subject>();
            cl = (from c in _db.Subjects select c).ToList();
            cl.Insert(0, new Subject { SubjId = 0, SubjectName = "--Select Subject--" });
            ViewBag.message = cl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(Assign obj)
        {
            if (ModelState.IsValid)
            {
                _db.Assign.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Assign");
            }
            return View(obj);
        }
        public IActionResult EnrollmentReport()
        {
            IEnumerable<EnrollmentReport> objList = _db.EnrollmentReports;
            return View(objList);
        }
    }
}
