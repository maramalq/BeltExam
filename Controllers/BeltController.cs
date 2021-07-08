using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
    public class BeltController : Controller
    {
        private MyContext _db;
        private int? uid
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
        }
        private bool isLoggedIn
        {
            get { return uid != null; }
        }
        public BeltController(MyContext context)
        {
            _db = context;
        }

        [HttpGet("home")]
        public IActionResult Dashboard()
        {
            if( !isLoggedIn )
            {
                return RedirectToAction("Index" , "Home"); 
            }

            List<Event> allEvents = _db.Events
                .Include(e => e.Coordainator) // one to many
                .Include(e => e.Participants) // many to many 
                .ThenInclude(p => p.Attendee)
                .Where(e => e.EventDate > DateTime.Today)
                .OrderBy(e => e.EventDate)
                //.OrderByDescending(e => e.EventTime)
                .ToList();

            User u = _db.Users.FirstOrDefault(u => u.UserId == (int)uid);

            Container container = new Container();
            container.User = u;
            container.Events = allEvents;

            return View(container);
        }

        [HttpGet("new")]
        public IActionResult NewEvent()
        {
            if( !isLoggedIn )
            {
                return RedirectToAction("Index" , "Home"); 
            }
            User u = _db.Users.FirstOrDefault(u => u.UserId == (int)uid);
            ViewBag.User = u;
            return View();
        }

        [HttpPost("create_event")]
        public IActionResult CreateEvent(Event newEvent)
        {
            if( !isLoggedIn )
            {
                return RedirectToAction("Index" , "Home"); 
            }

            if( newEvent.EventDate < DateTime.Now )
            {
                ModelState.AddModelError("EventDate" , "Activity can't be schulde in the past!!");
            }
            // run validation
            if (ModelState.IsValid)
            {
                newEvent.UserId = (int)uid;
                _db.Events.Add(newEvent);
                _db.SaveChanges();
                Console.WriteLine("added activity to db");
                return Redirect($"/activity/{newEvent.EventId}");
                //return RedirectToAction("EventInfo" , );
            }

            // User u = _db.Users.FirstOrDefault(u => u.UserId == (int)uid);
            // ViewBag.User = u;
            Console.WriteLine("something happen!");
            return View("NewEvent");
        }

        [HttpGet("activity/{eventId}")]
        public IActionResult EventInfo(int eventId)
        {
            if( !isLoggedIn )
            {
                return RedirectToAction("Index" , "Home"); 
            }

            Event thisEvent = _db.Events
                .Include(e => e.Coordainator) // grab the creator of activity
                .Include(e => e.Participants) //
                .ThenInclude(p => p.Attendee)
                .FirstOrDefault(e => e.EventId == eventId);

            Container container = new Container();
            User u = _db.Users.FirstOrDefault(u => u.UserId == (int)uid);
            container.User = u;
            container.Event = thisEvent;

            return View(container);
        }

        [HttpGet("delete/{eventId}")]
        public IActionResult DeleteEvent(int eventId)
        {
            if( !isLoggedIn )
            {
                return RedirectToAction("Index" , "Home");
            }
            // query events db by id
            Event deleted = _db.Events.FirstOrDefault(e => e.EventId == eventId);
            // remove from db
            _db.Events.Remove(deleted);
            // save changes
            _db.SaveChanges();
            Console.WriteLine("deleted!!");
            return RedirectToAction("Dashboard");
        }

        [HttpGet("join/{eventId}")]
        public IActionResult Join(int eventId)
        {
            // create RSVP instance
            RSVP rsvp = new RSVP();
            // reassign UserId and EventId
            rsvp.UserId = (int)uid;
            rsvp.EventId = eventId;
            // Add to RSVP Table in db
            _db.RSVPs.Add(rsvp);
            // save Changes
            _db.SaveChanges();
            // redirect dashboard
            Console.WriteLine("joined activity");
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{eventId}")]
        public IActionResult Leave(int eventId)
        {
            // query RSVP from db
            // must match event id and user id in the RSVP relationship
            RSVP unRsvp = _db.RSVPs
                .FirstOrDefault(r => r.AttendeeOf.EventId == eventId && r.Attendee.UserId == (int)uid );
            // remove the Like  from Table in db
            _db.RSVPs.Remove(unRsvp);
            // save Changes
            _db.SaveChanges();
            // redirect dashboard
            Console.WriteLine("Leaved activity");
            return RedirectToAction("Dashboard");
        }


    }
}