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

        public EventController(EtsDbContext context, Session session, EventService eventService, TicketService ticketService)
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
                    Description = model.EventDescription,
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

                return RedirectToAction("Index", "Events");
            }

            return View(model);
        }

        public IActionResult ReturnView(int eventId)
        {
            var eventDetails = _eventService.GetEventById(eventId);

            if (eventDetails == null)
            {
                return NotFound();
            }

            var currentUser = _session.GetCurrentUser();
            var currentUserId = currentUser?.Id ?? -1;

            if (eventDetails.CreatorId == currentUserId)
            {
                return OrganizerView(eventId);
            }

            return UserView(eventId);
        }

        public IActionResult OrganizerView(int eventId)
        {
            var eventDetails = _eventService.GetEventById(eventId);

            if (eventDetails == null)
            {
                return NotFound();
            }

            var ticketTypes = _ticketService.GetTicketTypesByEventId(eventId);

            var model = new EventViewModel
            {
                Event = eventDetails,
                TicketTypes = ticketTypes,
                TicketsSold = $"Tickets Sold: {_ticketService.GetEventTicketsSold(eventId)}",
                Revenue = $"Revenue: {_eventService.GetEventRevenue(eventId)}",
                StartDate = eventDetails.StartTime?.Date ?? DateTime.Now.Date,
                StartTime = eventDetails.StartTime ?? DateTime.Now,
                EndDate = eventDetails.EndTime?.Date ?? DateTime.Now.Date,
                EndTime = eventDetails.EndTime ?? DateTime.Now
            };

            return View("OrganizerView", model);
        }

        public IActionResult UserView(int eventId)
        {
            var eventDetails = _eventService.GetEventById(eventId);

            if (eventDetails == null)
            {
                return NotFound();
            }

            var ticketTypes = _ticketService.GetTicketTypesByEventId(eventId);

            var model = new EventViewModel
            {
                Event = eventDetails,
                TicketTypes = ticketTypes,
                TicketsSold = "",
                Revenue = "",
                StartDate = eventDetails.StartTime?.Date ?? DateTime.Now.Date,
                StartTime = eventDetails.StartTime ?? DateTime.Now,
                EndDate = eventDetails.EndTime?.Date ?? DateTime.Now.Date,
                EndTime = eventDetails.EndTime ?? DateTime.Now
            };

            return View("UserView", model);
        }

        [HttpPost]
        public IActionResult Save(CreateEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event eventEntity;

                if (model.EventId == 0)
                {
                    eventEntity = new Event
                    {
                        Name = model.EventName,
                        Location = model.EventLocation,
                        Description = model.EventDescription,
                        StartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartTime.Hours, model.StartTime.Minutes, 0),
                        EndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndTime.Hours, model.EndTime.Minutes, 0),
                        CreatorId = _session.GetCurrentUser().Id
                    };

                    _context.Events.Add(eventEntity);
                }
                else
                {
                    eventEntity = _context.Events.Find(model.EventId);
                    if (eventEntity == null)
                    {
                        return NotFound();
                    }

                    eventEntity.Name = model.EventName;
                    eventEntity.Location = model.EventLocation;
                    eventEntity.Description = model.EventDescription;
                    eventEntity.StartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartTime.Hours, model.StartTime.Minutes, 0);
                    eventEntity.EndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndTime.Hours, model.EndTime.Minutes, 0);
                }

                _context.SaveChanges();

                return RedirectToAction("OrganizerView", new { eventId = eventEntity.Id });
            }

            return View(model);
        }
    }
}