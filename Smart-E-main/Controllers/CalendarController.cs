using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;

namespace Smart_E.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<JsonResult> GetEvents()
        {
            var getALLEvents = await (
                from c in _context.Calendars
                select new
                {
                    c.Id,
                    c.Description,
                    c.End,
                    c.Start

                }).ToListAsync();

            return Json(getALLEvents);

            /*using (Calendar dc = new Calendar())
            {
                var events = dc.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }*/
        }
        /*
        [HttpPost]
        public JsonResult SaveEvent(Calendar e)
        {
            var status = false;
            /*using (Calendar dc = new Calendar())
            {
                if (e.Id > 0)
                {
                    //Update the event
                    var v = await _context.Calendars.Where(a => a.Id == e.Id).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDayEvent;
                        v.ThemeColor = e.Theme;
                    }
                }
                else
                {
                    _context.Calendars.Add(e);
                }

                _context.SaveChanges();
                status = true;

            //}
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            /*using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = dc.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            //}
            return new JsonResult { Data = new { status = status } };
        }*/
    }
}
