using System.ComponentModel;
using DocumentFormat.OpenXml.Office2013.PowerPoint;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.ChatRoom;
using Smart_E.Models.Forums;

namespace Smart_E.Controllers
{
    public class MyForumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyForumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult MyForums()
        {
            return View();
        }
        public IActionResult ParentForums()
        {
            return View();
        }
        public async Task<IActionResult> AllParentForums()
        {
            var user = await _userManager.GetUserAsync(User);
            var myForums = await (
                from t in _context.TeacherForums
                join u in _context.Users
                    on t.TeacherId equals u.Id
                where t.ParentId == user.Id 
                select new
                {
                    Id = t.Id,
                    Message = t.Message,
                    TeacherId = t.TeacherId,
                    ParentId = u.Id,
                    TeacherName = u.FirstName + " "+ u.LastName,
                    Date = t.Date,
                }).OrderBy(x=>x.Date).ToListAsync();

            return Json(myForums);
        }
        [HttpGet]
        public async Task<IActionResult> GetTeacherMessage([FromQuery] Guid id)
        {
            var forum = await _context.TeacherForums.SingleOrDefaultAsync(x => x.Id == id);
            if (forum != null)
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == forum.TeacherId);

                return Json(new
                {
                    Id = forum.Id,
                    Message = forum.Message,
                    Teacher = user.FirstName + " " + user.LastName
                });

            }

            return BadRequest("Forum not found");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateMessageToRead([FromQuery] Guid id)
        {
            var forum = await _context.TeacherForums.SingleOrDefaultAsync(x => x.Id == id);
            if (forum != null)
            {
                forum.ParentReadStatus = true;

                 _context.TeacherForums.Update(forum);
                 await _context.SaveChangesAsync();

                 return Json(forum);
            }

            return BadRequest("Forum not found");
        }
        [HttpPost]
        public async Task<IActionResult> SendParentMessage([FromBody] SendMessageToParent model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var date = DateTime.Now;
                var comment = new TeacherForums()
                {
                    Id = Guid.NewGuid(),
                    Message = model.Message,
                    Date = date,
                    TeacherId = currentUser.Id,
                    TeacherSentStatus = true,
                    ParentReadStatus = false,
                    ParentId = model.Parent
                };
                await _context.TeacherForums.AddAsync(comment);

                await _context.SaveChangesAsync();

                return Json(comment);
            }

            return BadRequest("Model is not valid");
        }
        [HttpGet]
        public async Task<IActionResult> GetParentReplyMessages()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                var invites = await (
                    from i in _context.TeacherForums
                    join u in _context.Users
                        on i.TeacherId equals u.Id
                    where i.ParentId == user.Id && i.ParentReadStatus == false && i.TeacherSentStatus == true
                    select new 
                    {
                        Id = i.Id,
                        Name = u.FirstName + " " + u.LastName,
                        Date = i.Date.ToString("g")
                    }).ToListAsync();

                return Json(invites);
            }

            return NotFound("User could not be found");

        }

        public async Task<IActionResult> AllMyForums()
        {
            var user = await _userManager.GetUserAsync(User);
            var myForums = await (
                from t in _context.TeacherForums
                join u in _context.Users
                    on t.ParentId equals u.Id
                where t.TeacherId == user.Id
                select new
                {
                    Id = t.Id,
                    Message = t.Message,
                    TeacherId = t.TeacherId,
                    ParentId = u.Id,
                    ParentName = u.FirstName + " "+ u.LastName,
                    Date = t.Date,
                }).OrderBy(x=>x.Date).ToListAsync();

            return Json(myForums);
        }

        public async Task<IActionResult> GetMyForum([FromQuery] Guid id )
        {
            var forum = await (
                from f in _context.TeacherForums
                join u in _context.Users
                    on f.ParentId equals u.Id
                    where f.Id == id
                select new
                {
                    Id = f.Id,
                    Message = f.Message,
                    ParentName = u.FirstName + " " + u.LastName,
                    ParentId  = f.ParentId

                }).SingleOrDefaultAsync();

            return Json(forum);

        }
        public async Task<IActionResult> GetParentForum([FromQuery] Guid id )
        {
            var forum = await (
                from f in _context.TeacherForums
                join u in _context.Users
                    on f.TeacherId equals u.Id
                where f.Id == id
                select new
                {
                    Id = f.Id,
                    Message = f.Message,
                    TeacherName = u.FirstName + " " + u.LastName,
                    ParentId  = f.ParentId

                }).SingleOrDefaultAsync();

            return Json(forum);

        }
    }
}
