using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.MyChild;
using Smart_E.Models.MyStudent;

namespace Smart_E.Controllers
{
    public class MyStudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public MyStudentsController( ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult MyStudents()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudentAttendance([FromBody] UpdateStudentAttendancePostModal modal)
        {
            if (ModelState.IsValid)
            {
                var myCourse = await _context.MyCourses.SingleOrDefaultAsync(x => x.Id == modal.Id);

                if (myCourse != null)
                {
                    myCourse.NumberOfClassesAttended = modal.NumberOfClassesAttended;

                    _context.MyCourses.Update(myCourse);

                    await _context.SaveChangesAsync();

                    return Json(myCourse);
                }

                return BadRequest("Course for this student not found");
            }

            return BadRequest("Modal is not valid");
        }

        [HttpPost]
        public async Task<IActionResult> HigherNumberOfClasses([FromQuery] Guid courseId, [FromQuery] int numberOfClassesAttended)
        {
            var course = await _context.Course.SingleOrDefaultAsync(x => x.Id == courseId);
            if (course!=null)
            {
                if (numberOfClassesAttended > course.NumberOfClasses)
                {
                    return BadRequest(
                        "You cannot exceed the amount of classes attended than the amount of classes that there are");
                }
                else
                {
                    return Json(course);
                }

            }
         
            return BadRequest("Course for this student not found");
        }

        public async Task<IActionResult> GetStudentAttendance([FromQuery] string studentId, [FromQuery] Guid courseId)
        {
            var course = await _context.Course.SingleOrDefaultAsync(x => x.Id == courseId);
            if (course != null)
            {
                var myCourse =
                    await _context.MyCourses.SingleOrDefaultAsync(x => x.CourseId == course.Id && x.StudentId == studentId);

                return Json(new
                {
                    Id = myCourse.Id,

                    NumberOfClassesAttended = myCourse.NumberOfClassesAttended,
                    NumberOfClasses = course.NumberOfClasses
                    
                });
            }

            return BadRequest("Course not found");
        }


        public async Task<IActionResult> MyStudentsProgress([FromQuery] string studentId, [FromQuery] Guid courseId)
        {
            var student = await _context.Users.SingleOrDefaultAsync(x => x.Id == studentId);

            if (student != null)
            {
                var course = await _context.Course.SingleOrDefaultAsync(x => x.Id == courseId);
                if (course != null)
                {
                    var myCourse =
                        await _context.MyCourses.SingleOrDefaultAsync(x =>
                            x.CourseId == course.Id && x.StudentId == student.Id);
                    
                    if (myCourse != null)
                    {
                        return View(new MyStudentsProgressViewModel()
                        {
                            Id = student.Id,
                            Name = student.FirstName + " " + student.LastName,
                            StudentEmail = student.Email,
                            CourseId = course.Id,
                            Grade = course.Grade,
                            CourseName = course.CourseName,
                            NumberOfClasses = course.NumberOfClasses,
                            NumberOfClassesAttended = myCourse.NumberOfClassesAttended,
                            //AttendancePercentage = ((myCourse.NumberOfClassesAttended / course.NumberOfClasses) * 100) + " %"
                    
                        });

                    }

                    return BadRequest("Course not found for this student");

                }
                return BadRequest("Course not found");

            }
            else
            {
                return View("Error");
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetAllMyStudentCourses()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var myStudents = await (
                from c in _context.Course
                join mc in _context.MyCourses
                    on c.Id equals mc.CourseId
                join u in _context.Users
                    on mc.StudentId equals u.Id
                where c.TeacherId == user.Id && mc.Status == true
                select new
                {
                    Id = mc.Id,
                    CourseId = c.Id,
                    CourseName = c.CourseName,
                    UserId = u.Id,
                    Email = u.Email,
                    TeacherId = c.TeacherId,
                    Grade = c.Grade,
                    StudentId = mc.StudentId,
                    Student = u.FirstName + " " + u.LastName
                }).ToListAsync();
           
            return Json(myStudents);
        }
    }
}
