using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smart_E.Models;
using Smart_E.Models.Courses;
using Smart_E.Models.Document;
using Smart_E.Models.SPClass;

namespace Smart_E.Controllers
{
    public class MyCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public MyCoursesController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;

        }
        public IActionResult MyCourses()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetAttachment(int ID)
        {
            byte[] fileContent;
            string fileName = string.Empty;
            Document document = new Document();
            document = _context.Documents.Select(m => m).Where(m => m.FileID == ID).FirstOrDefault();

            string contentType = SpClass.GetContenttype(document.FileName);
            fileContent = (byte[])document.attachment;
            return new FileContentResult(fileContent, contentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCourse([FromQuery] Guid id)
        {
            var course = await _context.Course.SingleOrDefaultAsync(x => x.Id == id);
            return Json(course);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMyCourse([FromBody] UpdateNumberOfClassesPostModal model)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Course.SingleOrDefaultAsync(x => x.Id == model.Id);

                if (course != null)
                {
                    course.NumberOfClasses = model.NumberOfClasses;

                    _context.Course.Update(course);
                    await _context.SaveChangesAsync();

                    return Json(course);
                }

                return BadRequest("Course not found");

            }

            return BadRequest("Model not found");

        }

        [HttpPost]

        public async Task<IActionResult> DeleteCourseInvite([FromQuery] Guid id)
        {
            var myInvite = await _context.MyCourses.SingleOrDefaultAsync(x => x.Id == id);

            if (myInvite != null)
            {
                _context.MyCourses.Remove(myInvite);
                await _context.SaveChangesAsync();

                return Json(myInvite);

            }

            return BadRequest("Course Invite not found");
        }
        [HttpPost]

        public async Task<IActionResult> UpdateStudentCourseInvite([FromQuery] Guid id)
        {
            var invite = await _context.MyCourses.SingleOrDefaultAsync(x => x.Id == id);

            if (invite != null)
            {
                invite.Status = true;
                _context.MyCourses.Update(invite);


                await _context.SaveChangesAsync();

                return Json(invite);
            }

            return BadRequest("Invite not found");

        }
        [HttpGet]
        public async Task<IActionResult> GetAllMyCoursesEnrollmentRequests()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var courses = await (
                from c in _context.Course
                join mc in _context.MyCourses
                    on c.Id equals mc.CourseId
                join u in _context.Users
                    on mc.StudentId equals u.Id
                where c.TeacherId == user.Id && mc.Status == false
                select new
                {
                    Id = mc.Id,
                    CourseId = c.Id,
                    CourseName = c.CourseName,
                    UserId = u.Id,
                    Email = u.Email,
                    StudentName = u.FirstName + " " + u.LastName
                }).ToListAsync();

            return Json(courses);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMyStudentCourses()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var courses = await (
                from c in _context.Course
                join mc in _context.MyCourses
                    on c.Id equals mc.CourseId
                join u in _context.Users
                    on c.TeacherId equals u.Id
                where mc.StudentId == user.Id && mc.Status == true
                select new
                {
                    Id = mc.Id,
                    CourseId = c.Id,
                    CourseName = c.CourseName,
                    UserId = u.Id,
                    Email = u.Email,
                    TeacherId = c.TeacherId,
                    TeacherName = u.FirstName + " " + u.LastName,
                    Grade = c.Grade
                }).ToListAsync();

            return Json(courses);
        }
        [HttpGet]
        public FileStreamResult GetFileStreamResultDemo(string filename) //download file
        {
            string path = "wwwroot/attachment/" + filename;
            var stream = new MemoryStream(System.IO.File.ReadAllBytes(path));
            string contentType = SpClass.GetContenttype(filename);
            return new FileStreamResult(stream, new MediaTypeHeaderValue(contentType))
            {
                FileDownloadName = filename
            };
        }

        [HttpGet]
        public FileContentResult GetFileContentResultDemo(string filename)
        {
            string path = "wwwroot/attachment/" + filename;
            byte[] fileContent = System.IO.File.ReadAllBytes(path);
            string contentType = SpClass.GetContenttype(filename);
            return new FileContentResult(fileContent, contentType);
        }
        [HttpGet]
        public VirtualFileResult GetVirtualFileResultDemo(string filename)
        {
            string path = "attachment/" + filename;
            string contentType = SpClass.GetContenttype(filename);
            return new VirtualFileResult(path, contentType);
        }

        [HttpGet]
        public PhysicalFileResult GetPhysicalFileResultDemo(string filename)
        {
            string path = "/wwwroot/attachment/" + filename;
            string contentType = SpClass.GetContenttype(filename);
            return new PhysicalFileResult(_hostingEnvironment.ContentRootPath
                                          + path, contentType);
        }

        [HttpGet]
        public FileResult GetFileResultDemo(string filename)
        {
            string path = "/attachment/" + filename;
            string contentType = SpClass.GetContenttype(filename);
            return File(path, contentType);
        }
        [HttpGet]
        public IActionResult UploadAttachment()
        {
            ViewBag.Action = "Upload";
            ViewBag.Chapter = _context.Chapter.Select(t => t).ToList();
            return View();
        }
        [HttpPost]
        public IActionResult UploadAttachment(ChapterViewModel model)
        {
            if (model.attachment != null)
            {
                //write file to a physical path
                var uniqueFileName = SpClass.CreateUniqueFileExtension(model.attachment.FileName);
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "attachment");
                var filePath = Path.Combine(uploads, uniqueFileName);

                model.attachment.CopyTo(new FileStream(filePath, FileMode.Create));

                ViewBag.Chapter = _context.Chapter.Select(t => t).ToList();
                //save the attachment to the database
                Document document = new Document();
                document.FileName = uniqueFileName;
                document.ChapterID = model.ChapterID;


                document.attachment = SpClass.GetByteArrayFromImage(model.attachment);

                _context.Documents.Add(document);

                _context.SaveChanges();

            }

            return RedirectToAction("AllMySubjects", "Courses");
        }

        public static byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }




    }
}
