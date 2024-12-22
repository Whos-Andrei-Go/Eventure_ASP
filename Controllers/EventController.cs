using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Services;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Eventure_ASP.Controllers
{
    public class EventController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session;

        private readonly EventService _eventService;
        private readonly TicketService _ticketService;

        public EventController(EtsDbContext context, Session session, EventService eventService, TicketService ticketService) // Inject the Session class
        {
            _context = context;
            _session = session;

            _eventService = eventService;
            _ticketService = ticketService;
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

        public IActionResult OrganizerView(int eventId)
        {
            // Fetch the event from the database
            var eventDetails = _eventService.GetEventById(eventId);

            if (eventDetails == null)
            {
                return NotFound(); // Handle the case where the event is not found
            }

            // Get the current user's ID
            var currentUser = _session.GetCurrentUser();
            var currentUserId = currentUser?.Id ?? -1;

            if (eventDetails.CreatorId != currentUserId)
            {
                return Forbid(); // Prevent access if the user is not the creator
            }

            // Fetch ticket types associated with the event
            var ticketTypes = _ticketService.GetTicketTypesByEventId(eventId);

            // Prepare the view model
            var model = new OrganizerEventViewModel
            {
                Event = eventDetails,
                TicketTypes = ticketTypes,
                TicketsSold = $"Tickets Sold: {_ticketService.GetEventTicketsSold(eventId)}",
                Revenue = $"Revenue: {_eventService.GetEventRevenue(eventId)}"
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveEvent(Event eventData)
        {
            // Logic to save the event data
            bool success = _eventService.SaveEvent(eventData);
            return Json(new { success });
        }
    }
}