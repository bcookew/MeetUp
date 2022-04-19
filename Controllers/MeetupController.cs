using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MeetUp.Models;


namespace MeetUp.Controllers
{
    public class MeetUpController : Controller
    {
        private readonly ILogger<MeetUpController> _logger;
        private MeetUpContext _context;

        public MeetUpController(ILogger<MeetUpController> logger, MeetUpContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ====================
        // ====================== Dashboard route
        // ====================

        [HttpGet("/meetups")]
        public IActionResult Dashboard()
        {
            if(LoggedIn())
            {
                List<Event> MeetUps = _context.Events
                                .Include(e => e.Creator)
                                .Include(e => e.Participants)
                                .Where(e => DateTime.Compare(DateTime.Now, e.EventStartTime) < 0)
                                .OrderBy(e => e.EventStartTime)
                                .ToList();
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.Dashboard = true;
                return View(MeetUps);
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
            
        }

        // ====================
        // ====================== New Meet Up form loader
        // ====================

        [HttpGet("/meetups/new")]
        public IActionResult NewMeetup()
        {
            if(LoggedIn())
            {
                return View();
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
        }
        
        // ====================
        // ====================== Add event
        // ====================
        [HttpPost("/meetups/new")]
        public IActionResult AddEvent(Event e)
        {
            e.UserId = (int)HttpContext.Session.GetInt32("UserId");
            if(ModelState.IsValid)
            {
                e.EventEndTime = e.EventStartTime + GetSpan(e.Duration, e.TimeMeasure);
                _context.Add(e);
                _context.SaveChanges();
                return RedirectToAction("ViewMeetup", new {id = e.EventId});
            }
            else
            {
                return View("NewMeetup");
            }
        }

        // ====================
        // ====================== View a meetup by ID
        // ====================

        [HttpGet("/meetups/{id}")]
        public IActionResult ViewMeetup(int id)
        {
            if(LoggedIn())
            {
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                Event e = _context.Events
                            .Include(e => e.Creator)
                            .Include(e => e.Participants)
                                .ThenInclude(p => p.Participant)
                            .SingleOrDefault(e => e.EventId == id);
                return View(e);
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
        }
        
        // ====================
        // ====================== Join Event
        // ====================
        public IActionResult Join(int id)
        {
            if(LoggedIn())
            {
                ParticipantAtEvent p = new ParticipantAtEvent();
                p.EventId = id;
                p.UserId = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(p);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
        }
        
        // ====================
        // ====================== Leave Event
        // ====================
        public IActionResult Leave(int id)
        {
            if(LoggedIn())
            {
                ParticipantAtEvent p = _context.ParticipantsAtEvents.SingleOrDefault(p => p.EventId == id && p.UserId == HttpContext.Session.GetInt32("UserId"));
                _context.Remove(p);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
        }
        
        // ====================
        // ====================== Delete Event
        // ====================
        public IActionResult Delete(int id)
        {
            if(LoggedIn())
            {
                Event e = _context.Events.SingleOrDefault(e => e.EventId == id && e.UserId == HttpContext.Session.GetInt32("UserId"));
                _context.Remove(e);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
        }

        // ====================== User Management and Validation Methods        
        private bool LoggedIn()
        { 
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private TimeSpan GetSpan(int Val, int measure)
        {
            switch (measure) 
            {
                case 1:
                    return TimeSpan.FromHours(Val);
                case 2:
                    return TimeSpan.FromMinutes(Val);
                case 3:
                    return TimeSpan.FromDays(Val);
            }
            return TimeSpan.Zero;
        }
    }
}
