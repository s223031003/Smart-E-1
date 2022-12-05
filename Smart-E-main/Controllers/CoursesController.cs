using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.Courses;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Smart_E.Controllers
{
    public class CoursesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public CoursesController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _userManager = userManager;
        }
        
        public IActionResult Courses()
        {
            var courses = _context.Course
                        .OrderBy(c => c.CourseName).ToList(); 
            return View(courses);
           
        }

        public async Task<IActionResult> GetAllMyAssignmentsForThisCourse([FromQuery] Guid courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var myAssignmentForThisCourse = await (
                from mc in _context.MyCourses
                join c in _context.Course
                    on mc.CourseId equals c.Id
                join a in _context.Assignments
                    on c.Id equals a.CourseId
                join ar in _context.AssignmentResults
                    on a.Id equals ar.AssignmentId
                where mc.Id == courseId && mc.StudentId == user.Id
                select new
                {
                    Id = a.Id,
                    CourseId = c.Id,
                    AssignmentName = a.Name,
                    AssignmentMark = a.Mark,
                    StudentId = mc.StudentId,
                    CourseName = c.CourseName,
                    NewMark = ar.NewMark,
                    Percentage = ((ar.NewMark / a.Mark) * 100) + " %",
                    Outcome =  ((ar.NewMark / a.Mark) * 100)<= 49 ? "FAIL" : "PASS" 
                }).ToListAsync();

            return Json(myAssignmentForThisCourse);
        }
        public IActionResult AllMySubjects() ///This is specific to just a teacher who can add chapters in a course 
        {
            return View();

        }
        public  async Task<IActionResult> GetAllMySubjects()
        {
            var user  = await _userManager.GetUserAsync(User);
            var courses = await (from c in _context.Course
                join u in _context.Users
                    on c.TeacherId equals u.Id
                    where c.TeacherId == user.Id
                select new
                {
                    Id = c.Id,
                    TeacherId = c.TeacherId,
                    CourseName = c.CourseName,
                    Grade = c.Grade,
                    TeacherName = u.FirstName + " " +u.LastName
                }).ToListAsync();

            return Json(courses);
           
        }

        public IActionResult MyCourses()
        {
            var myCourses = _context.Course
                .OrderBy(c => c.CourseName).ToList(); 
            return View(myCourses);
           
        }

        public async Task<IActionResult> EnrollIntoCourse([FromQuery] Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Course.SingleOrDefaultAsync(x => x.Id == id);

            if (course != null)
            {
                var myCourse = new MyCourses()
                {
                    Id = Guid.NewGuid(),
                    StudentId = user.Id,
                    CourseId = course.Id,
                };
                await _context.MyCourses.AddAsync(myCourse);
                await _context.SaveChangesAsync();

                return Json(course);

            }

            return BadRequest("Course not found");

        }
        
        public async Task<IActionResult> CourseDetails([FromQuery] Guid id)
        {
            var myCourses = await _context.Course.SingleOrDefaultAsync(x => x.Id == id);

            if (myCourses != null)
            {
                ChapterViewModel chapterViewModel = new ChapterViewModel();
          
                chapterViewModel.chapters = _context.Chapter.Where(x=>x.CourseId == myCourses.Id).OrderBy(c=> c.ChapterName).ToList();
                chapterViewModel.documents = _context.Documents.Select(d => d).ToList();

                chapterViewModel.Course = myCourses.CourseName;
                chapterViewModel.Grade = myCourses.Grade;

           
                return View(chapterViewModel);
            }

            return BadRequest("Courses not found");

        }

        public async Task<IActionResult> GetAllTeachers()
        {
            var teacher = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                    where r.Name == "Teacher"
                select new
                {
                    Id = u.Id,
                    Name= u.FirstName + " " + u.LastName,


                }).ToListAsync();

            return Json(teacher);
        }


        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await (
                from c in _context.Course
                join u in _context.Users
                    on c.TeacherId equals u.Id
                select new
                {
                    Id = c.Id,
                    CourseName = c.CourseName,
                    Grade = c.Grade,
                    TeacherName = u.FirstName + " " + u.LastName

                }).OrderBy(x=>x.Grade).ToListAsync();
           
            return Json(courses);
        }

        [HttpGet]
        public IActionResult CreateChapter()
        {
            ViewBag.Action = "Create";
            ViewBag.Course = _context.Course.OrderBy(t => t.CourseName).ToList();
            return View();
        }
        [HttpGet]
      

        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCoursePostModel model)
        {
            if (ModelState.IsValid)
            {
                var existingCourse = await _context.Course.SingleOrDefaultAsync(x => x.CourseName == model.CourseName && x.Grade == model.Grade);

                if (existingCourse == null)
                {

                    var existingTeacher = await _context.Users.SingleOrDefaultAsync(x => x.Id == model.TeacherName);

                    if (existingTeacher != null)
                    {
                        var course = new Course()
                        {
                            Id = Guid.NewGuid(),
                            CourseName = model.CourseName,
                            TeacherId = model.TeacherName,
                            Grade = model.Grade,
                            NumberOfClasses = 1
                        };

                        await _context.Course.AddAsync(course);

                        await _context.SaveChangesAsync();

                        return Json(course);
                    }
                    return BadRequest("Please assign a teacher to this course");
                }

                return BadRequest("This Course already Exists");

            }
            return BadRequest("Please fill in all required fields");
        }
        [HttpPost]
        public IActionResult CreateChapter(ChapterViewModel chapterViewModel)
        {

            if (chapterViewModel != null)
            {

                var chapters = new List<Chapter>
                {
                    new Chapter{ChapterID= Guid.NewGuid(), ChapterName=chapterViewModel.ChapterName, Date = DateTime.Now, Description = chapterViewModel.Description,CourseId = chapterViewModel.Id}
                };


                chapters.ForEach(s => _context.Chapter.Add(s));

                _context.SaveChanges();
                ViewBag.Message = "Data saved successfully.";

                return RedirectToAction("AllMySubjects", "Courses");

            }
            ViewBag.Action = " Create";
            ViewBag.Message = "Error while saving record.";
            return RedirectToAction("CourseDetails", "Courses", new { id = chapterViewModel.Id });
        }
       

       


    }
}
