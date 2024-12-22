using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventure_ASP.Controllers
{
    public class EventsController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session;

        public EventsController(EtsDbContext context, Session session) // Inject the Session class
        {
            _context = context;
            _session = session; // Initialize the session field
        }

        public IActionResult Index()
        {
            var events = _context.Events
                .Include(e => e.Creator)
                .ToList();

            return View(events);
        }

        public IActionResult Create()
        {
            return View(new CreateEventViewModel());
        }

        [HttpPost]
        public IActionResult Create(CreateEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventEntity = new Event
                {
                    Name = model.EventName,
                    Location = model.EventLocation,
                    Description = model.Description,
                    StartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartTime.Hours, model.StartTime.Minutes, 0),
                    EndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndTime.Hours, model.EndTime.Minutes, 0),
                    CreatorId = _session.GetCurrentUser().Id
                };

                _context.Events.Add(eventEntity);
                _context.SaveChanges();

                foreach (var ticketType in model.TicketTypes)
                {
                    var ticketEntity = new TicketType
                    {
                        EventId = eventEntity.Id,
                        Name = ticketType.Name,
                        Price = ticketType.Price,
                        Quantity = ticketType.Quantity
                    };

                    _context.TicketTypes.Add(ticketEntity);
                }

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}