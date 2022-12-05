using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.ChatRoom;

namespace Smart_E.Controllers
{
    public class ChatHubController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ChatHubController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult ChatHub()
        {
            return View();
        }

        public async Task<IActionResult> GetChats()
        {
            var comments= await (
                from c in _context.ChatRoom
                join u in _context.Users
                    on c.UserId equals u.Id
                select new
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    Date = c.DateTime.ToString("D"),
                    Name = u.FirstName + " " + u.LastName
                }).ToListAsync();

            return Json(comments);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentPostModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var date = DateTime.Now;
                var comment = new ChatRoom()
                {
                    Id = Guid.NewGuid(),
                    Comment = model.Comment,
                    DateTime = date,
                    UserId = currentUser.Id
                };
                await _context.ChatRoom.AddAsync(comment);

                await _context.SaveChangesAsync();

                return Json(comment);
            }

            return BadRequest("Model is not valid");
        }
    }
}
