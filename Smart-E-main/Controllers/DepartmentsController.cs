using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;
using ClosedXML;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.AllUsers;
using Smart_E.Models.Departments;

namespace Smart_E.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public DepartmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AllDepartments()
        {

            var department = await (from d in _context.Departments
                join u in _context.Users
                    on d.HODId equals u.Id
                select new
                {
                    Id = d.Id,
                    DepartmentName = d.DepartmentName,
                    HodName = u.FirstName + " "+ u.LastName,
                    HodId = d.HODId

                }).ToListAsync();

            return Json(department);

        }
        public async Task<IActionResult> AllMyDepartments()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);


            var department = await (
                from d in _context.Departments
                join u in _context.Users
                    on d.HODId equals u.Id
                where d.HODId == user.Id
                select new
                {
                    Id = d.Id,
                    DepartmentName = d.DepartmentName,
                    HodName = u.FirstName + " "+ u.LastName,
                    HodId = d.HODId

                }).ToListAsync();

            return Json(department);

        }

        [HttpPost]
        public async Task<IActionResult> AddSubjectToDepartment([FromQuery] Guid id, [FromQuery] Guid subjectId)
        {
            var department = await _context.Departments.SingleOrDefaultAsync(x => x.Id == id);

            if (department != null)
            {
                var subject = await _context.Course.SingleOrDefaultAsync(x => x.Id == subjectId);

            if (subject != null)
            {
                var departSub = new DepartmentSubjects()
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = department.Id,
                    CourseId = subject.Id
                };
                await _context.AddAsync(departSub);
                await _context.SaveChangesAsync();

                return Json(departSub);

            }
            return BadRequest("Subject not found");
            }

            return BadRequest("Department not found");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSubjectFromDepartment([FromQuery] Guid id)
        {
            var departmentSub = await _context.DepartmentSubjects.SingleOrDefaultAsync(x => x.Id == id);

            if (departmentSub != null)
            {
                _context.DepartmentSubjects.Remove(departmentSub);
                await _context.SaveChangesAsync();


                return Json(departmentSub);
            }
            return BadRequest("Department not found");
        }

        public async Task<IActionResult> GetSubjectDepartment([FromQuery] Guid id)
        {
             var dept = await (
                 from d in _context.Departments
                 join ds in  _context.DepartmentSubjects
                    on d.Id equals ds.DepartmentId 
                    join c in _context.Course
                     on ds.CourseId equals c.Id
                 where d.Id == id
                 select new
                 {
                     Id = ds.Id,
                     DepartmentId = d.Id,
                     SubjectId = c.Id,
                     SubjectName = c.CourseName + " " + c.Grade

                 }).ToListAsync();

             return Json(dept);


        }
        public async Task<IActionResult> GetAllSubjects()
        {
            var subject = await (from c in _context.Course

                                 select new
                                 {
                                     SubjectId = c.Id,
                                     Subject = c.CourseName + " " + c.Grade

                                 }).ToListAsync();

            return Json(subject);

        }


        public async Task<IActionResult> DepartmentDetails([FromQuery] Guid id)
        {
             var department = await _context.Departments.SingleOrDefaultAsync(x => x.Id == id);

             if (department != null)
             {
                 var deptName = await (
                     from d in _context.Departments
                     where d.Id == id
                     select new DepartmentViewModel()
                     {
                         Id = d.Id,
                         DepartmentName = d.DepartmentName

                     }).SingleOrDefaultAsync();

                 return View(deptName);
             }

             return BadRequest("Department Not Found");

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentPostModal modal)
        {
            if (ModelState.IsValid)
             {
                 var depart = new Departments()
                 {
                     Id = Guid.NewGuid(),
                     DepartmentName = modal.Name,
                     HODId = modal.Hod,
                 };
                 await _context.Departments.AddAsync(depart);
                 await _context.SaveChangesAsync();

                  return Json(depart);

             }

             return BadRequest("Modal not valid");
        }

      
        public IActionResult Departments()
        {
            return View();

        }
        public IActionResult MyDepartments()
        {
            return View();

        }
        
        public async Task<IActionResult> GetHODs()
        {

            var hod = await (
                from u in _context.Users
                join ur in _context.UserRoles
                on u.Id equals ur.UserId
                join r in _context.Roles
                on ur.RoleId equals r.Id
                where r.Name == "HOD"
                select new
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,

                }).ToListAsync();

            return Json(hod);

        }
        public async Task<IActionResult> GetSubjects()
        {

            var subjects = await (
                from c in _context.Course
                select new
                {
                    Id = c.Id,
                    Name = c.CourseName,

                }).ToListAsync();

            return Json(subjects);

        }
    }
}
