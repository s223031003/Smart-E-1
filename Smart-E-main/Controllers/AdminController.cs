using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Data;

namespace Smart_E.Controllers
{
    //[Authorize(Roles ="Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
        {
            _context = context;
            _roleManager = roleManager;
        }
        private List<ApplicationUser> users = new List<ApplicationUser>();
       /* public IActionResult Index()
        {
            return View();
        }*/

        [HttpPost]
        public FileResult Export()
        {
            DataTable dt = new DataTable("Users");
            dt.Columns.AddRange(new DataColumn[4]
            {
                new DataColumn("Name"),
                new DataColumn("Email"),
                new DataColumn("Role"),
                new DataColumn("Status")
            });

            var users =  (
                from c in _context.Users
                join ur in _context.UserRoles
                    on c.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                select new
                {
                    Id = c.Id,
                    Name = c.FirstName + " "+ c.LastName,
                    Email = c.Email,
                    Role = r.Name,
                    Status = c.Status

                }).ToList();
            foreach (var u in users)
            {
                dt.Rows.Add(u.Name, u.Email, u.Role, u.Status);
            }


            using(XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using(MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml", "Users.xlsx");
                }
            }
        }
        //search user
        //[HttpGet]
        //public async Task<IActionResult> Dashboard(string search) 
        //{
        //    ViewData["Details"] = search;

        //    var query = from u in _context.Users select u;
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        query = query.Where(s => s.FirstName.Contains(search) || s.Email.Contains(search));
        //    }
        //    return View(await query.AsNoTracking().ToListAsync());
       //}

        //retrieves data from database
        /*public IActionResult Admin()
        {
            IEnumerable<ApplicationUser> userList = _context.Users;
            return View(userList);
        }*/
        // GET: Users/AddOrEdit
        // GET: Users/AddOrEdit/5
        public async Task<IActionResult> AddOrEditUser(string id)
        {
            if (id == null)
                return View(new ApplicationUser());
            else
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
        }

        // POST: Users/AddOrEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditUser(string id, [Bind("Id,FirstName,LastName,Email,Role,Status")] ApplicationUser user)
        {

            if (ModelState.IsValid)
            {
                if (id == " ")
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ApplicationUserExists(user.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllUsers", _context.Users.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEditUser", user) });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (ModelState.IsValid)
            {
                var existingRole = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

                if (existingRole != null)
                {
                    _context.Users.Remove(existingRole);
                    _context.SaveChanges();
                }

                return BadRequest("This User does not exist");

            }
            return BadRequest("Model is not valid");
        }
        public IActionResult GetTeachers()
        {
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> GetTeachers()
        //{
        //    var teachers = await (
        //        from u in _context.TeachersReport               
        //        select new
        //        {
        //            Id = u.Id,
        //            Name = u.Name,
        //            Email = u.Email,
        //            Grade = u.Grade,
        //            Qualification = u.Qualification,
        //            Subjects = u.Subjects,
        //            TargetsAchieved = u.TargetsAchieved,
        //            Role = u.Name,
        //            Status = u.Status
        //        }).ToListAsync();

        //    return Json(teachers);
        //}
        public IActionResult GetHODs()
        {
            return View();
        }
        public async Task<IActionResult> GetHOD()
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
                    Email = u.Email,
                    Role = r.Name

                }).ToListAsync();

            return Json(hod);
        }
        public IActionResult GetStudents()
        {
            return View();
        }
        public async Task<IActionResult> GetStudent()
        {
            var student = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                on ur.RoleId equals r.Id
                where r.Name == "Student"
                select new
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Role = r.Name

                }).ToListAsync();

            return Json(student);
        }
        public IActionResult GetParents() {
                return View();
        }
        public async Task<IActionResult> GetParent()
        {
            var parent = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                on ur.RoleId equals r.Id
                where r.Name == "Parent"
                select new
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    //Status = u.Status,
                    Role = r.Name

                }).ToListAsync();

            return Json(parent);
        }

        //Get method for deleting a user
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _context.Users.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Post method for deleting a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(ApplicationUser obj)
        {
            var user =  _context.Users.Find(obj.Id);
            _context.Users.Remove(user);
            _context.SaveChanges();
                    return RedirectToAction("Dashboard");
        }
        //Get method for finding a user
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _context.Users.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
