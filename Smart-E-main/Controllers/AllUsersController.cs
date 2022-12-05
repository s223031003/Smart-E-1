using System.Threading.Tasks.Dataflow;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.AllUsers;

namespace Smart_E.Controllers
{
    public class AllUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AllUsersController(ApplicationDbContext context ,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeachers()
        {
            var teachers = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                where r.Name == "Teacher"
                select new
                {
                    Id = u.Id,
                    TeacherName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                }).ToListAsync();
            return Json(teachers);


        }
        public async Task<IActionResult>GetChildren()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var children = await (
                from i in _context.Invites
                join u in _context.Users
                    on i.InviteFrom equals u.Id
                where i.Status == true && i.InviteTo == user.Id
                select new
                {
                    Id = i.InviteFrom,
                    Name = u.FirstName + " "+ u.LastName,

                }).ToListAsync();

            return Json(children);

        }
        public async Task<IActionResult> GetAllThisTeachersSubjects([FromQuery] string id)
        {
            var teacher = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (teacher != null)
            {
                var theirInfo = await (
                    from c in _context.Course
                    join u in _context.Users
                        on c.TeacherId equals u.Id
                    where c.TeacherId ==teacher.Id 
                    select new 
                    {
                        Id = c.Id,
                        SubjectName = c.CourseName,
                        
                    }).ToListAsync();

                return Json(theirInfo);

            }
            return BadRequest("Student not found");

        }
        public async Task<IActionResult> GetTeacherQualification([FromQuery] string id)
        {
            var teacher = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (teacher != null)
            {
                var theirInfo = await (
                    from c in _context.Qualifications
                    join u in _context.Users
                        on c.UserId equals u.Id
                    where c.UserId ==teacher.Id 
                    select new 
                    {
                        Id = c.Id,
                        Description = c.Description,
                        QualificationType= c.QualificationType,
                        SchoolName=c.SchoolName,
                        YearAchieved=c.YearAchieved
                        
                    }).ToListAsync();

                return Json(theirInfo);

            }
            return BadRequest("Student not found");

        }
        public async Task<IActionResult> GetAllThisStudentSubjects([FromQuery] string id)
        {
            var student = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (student != null)
            {
                var theirInfo = await (
                    from mc in _context.MyCourses
                        join c in _context.Course
                        on mc.CourseId equals c.Id
                    join u in _context.Users
                        on c.TeacherId equals u.Id
                    where mc.StudentId ==student.Id && mc.Status == true
                    select new 
                    {
                        Id = mc.Id,
                        SubjectName = c.CourseName,
                        TeacherName = u.FirstName + " "+ u.LastName,
                        Grade = c.Grade
                    }).ToListAsync();

                return Json(theirInfo);

            }
            return BadRequest("Student not found");

        }

        public async Task<IActionResult> HODDetails([FromQuery] string id)
        {
            var hod = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (hod != null)
            {
                var theirInfo = await (
                    from u in _context.Users
                    where u.Id == hod.Id
                    select new ParentDetailsViewModel()
                    {
                        Id = u.Id,
                        Name = u.FirstName + " "+ u.LastName,
                        Email = u.Email

                    }).SingleOrDefaultAsync();
           
                return View(theirInfo);

            }
            return BadRequest("HOD not found");
        }

        public async Task<IActionResult> StudentDetails([FromQuery] string id)
        {
            var student = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (student != null)
            {

                var theirInfo = await (
                    from u in _context.Users
                    where u.Id == student.Id
                    select new StudentDetailsViewModel
                    {
                        Id = u.Id,
                        Name = u.FirstName + " "+ u.LastName,
                        Email = u.Email

                    }).SingleOrDefaultAsync();
           
                return View(theirInfo);
            }

            return BadRequest("Student not found");

        }
        public async Task<IActionResult> TeacherDetails([FromQuery] string id)
        {
            var teacher = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (teacher != null)
            {

                var theirInfo = await (
                    from u in _context.Users
                    where u.Id == teacher.Id
                    select new ParentDetailsViewModel()
                    {
                        Id = u.Id,
                        Name = u.FirstName + " "+ u.LastName,
                        Email = u.Email

                    }).SingleOrDefaultAsync();
           
                return View(theirInfo);
            }

            return BadRequest("Parent not found");

        }
        public async Task<IActionResult> ParentDetails([FromQuery] string id)
        {
            var parent = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (parent != null)
            {

                var theirInfo = await (
                    from u in _context.Users
                    where u.Id == parent.Id
                    select new ParentDetailsViewModel()
                    {
                        Id = u.Id,
                        Name = u.FirstName + " "+ u.LastName,
                        Email = u.Email

                    }).SingleOrDefaultAsync();
           
                return View(theirInfo);
            }

            return BadRequest("Parent not found");

        }

        public IActionResult Teachers()
        {
            return View();
        }
       
        public IActionResult Students()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                where r.Name == "Student"
                select new
                {
                    Id = u.Id,
                    StudentName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Role = r.Name

                }).ToListAsync();
            return Json(students);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHods()
        {
            var hods = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                    join d in _context.Departments 
                    on u.Id equals d.HODId
                where r.Name == "HOD"
                select new
                {
                    Id = u.Id,
                    HodName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    DepartmentName = d.DepartmentName
                }).ToListAsync();
            return Json(hods);
        }
        public IActionResult Parents()
        {
            return View();
        }
        public IActionResult HODs()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllParents()
        {
            var parents = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                where r.Name == "Parent"
                select new
                {
                    Id = u.Id,
                    ParentName = u.FirstName + " " + u.LastName,
                    Email = u.Email,

                }).ToListAsync();
            return Json(parents);

        }
    }
}
