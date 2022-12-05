using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;
using Smart_E.Models.Invites;

namespace Smart_E.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public InvitesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInvites()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                var invites = await (
                    from i in _context.Invites
                    join u in _context.Users
                    on i.InviteFrom equals u.Id
                   where i.InviteTo == user.Id && i.Status == false
                    select new 
                    {
                        Id = i.Id,
                        Name = u.FirstName + " " + u.LastName,
                        Date = i.CreationDate.ToString("yy-MM-dd"),
                    }).ToListAsync();

                return Json(invites);
            }

            return NotFound("User could not be found");

        }

        [HttpPost]

        public async Task<IActionResult> DeleteParentInvite([FromQuery] Guid id)
        {
            var invite = await _context.Invites.SingleOrDefaultAsync(x => x.Id == id);

            if (invite != null)
            {
                _context.Invites.Remove(invite);
                await _context.SaveChangesAsync();

                return Json(invite);

            }

            return BadRequest("Invite not found");
        }

        [HttpPost]

        public async Task<IActionResult> UpdateParentInvite([FromQuery] Guid id)
        {
            var invite = await _context.Invites.SingleOrDefaultAsync(x => x.Id == id);

            if (invite != null)
            {
                invite.Status = true;
                _context.Invites.Update(invite);


                await _context.SaveChangesAsync();

                return Json(invite);
            }

            return BadRequest("Invite not found");

        }

        [HttpPost]
        public async Task<IActionResult> AddParentInvite([FromBody] CreateParentInvitePostModal model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var date = DateTime.Now;

                var userTo = await _context.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

                if (userTo !=null)
                {

                    var inviteUser = await _context.Users.SingleOrDefaultAsync(x => x.Email == model.Email && x.Id == userTo.Id );

                    if (inviteUser != null)
                    {
                        var inviteUserRole =
                            await _context.UserRoles.SingleOrDefaultAsync(x => x.UserId == inviteUser.Id);

                        if (inviteUserRole != null)
                        {
                            var parent = await _context.Roles.SingleOrDefaultAsync(x => x.Id == inviteUserRole.RoleId);

                            if (parent.Name == "Parent")
                            {
                                var invite= new Invite()
                                {
                                    Id = Guid.NewGuid(),
                                    InviteFrom = user.Id,
                                    InviteTo = userTo.Id,
                                    CreationDate = date,
                                    Status = false,
                                    Message = ""

                                };
                                await _context.Invites.AddAsync(invite);

                                await _context.SaveChangesAsync();

                                return Json(invite);
                            }
                            return BadRequest("This user is not a parent");
                        }

                    }
                    return BadRequest("You have already sent an invitation to this user");

                    
                }
                return BadRequest("There is no account with that email on our system.");
            }
            return BadRequest("Model is not valid");
        }


    }
}
