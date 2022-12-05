using Microsoft.AspNetCore.Mvc;
using Smart_E.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;

namespace Smart_E.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Student()
        {
            var student = await _userManager.GetUserAsync(User);
            ViewBag.TotalSubject = _context.MyCourses.Where(c => c.StudentId == student.Id && c.Status == true).Count();

            var username = User.Identity.Name;
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            string firstName = string.Concat(new string[] { user.FirstName });
            ViewBag.FirstName = firstName;
            string lastName = string.Concat(new string[] { user.LastName });
            ViewBag.LastName = lastName;
            string email = string.Concat(new string[] { user.Email });
            ViewBag.Email = email;
            string phoneNumber = string.Concat(new string[] { user.PhoneNumber });
            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }
        public async Task<IActionResult> Parent()
        {
            var parent = await _userManager.GetUserAsync(User);
            ViewBag.TotalInvites = _context.Invites.Where(c => c.InviteTo == parent.Id && c.Status == false).Count();
            ViewBag.TotalChildren = _context.Invites.Where(c => c.InviteTo == parent.Id && c.Status == true).Count();
            ViewBag.TotalChats =  _context.TeacherForums.Where(c => c.ParentId == parent.Id && c.ParentReadStatus == false && c.TeacherSentStatus == true).Count();
            return View();
        }
        public IActionResult Admin()
        {
            IEnumerable<ApplicationUser> userList = _context.Users;
            return View(userList);
        }
        public IActionResult HOD()
        {
            return View();
        }

       public IActionResult Teacher()
        {
            ViewBag.TotalParents = _context.UserRoles.Where(c => c.RoleId == "8f18ff88-e58e-4ee6-8ee0-82072a1e6d76").Count();
            ViewBag.TotalLearners = _context.UserRoles.Where(c => c.RoleId == "8d709b3e-7d72-4451-8d3d-340e527d1c16").Count();
            ViewBag.TotalSubjects = _context.Course.Count();
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}