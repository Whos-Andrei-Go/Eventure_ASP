using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Services;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


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
        [HttpPost]
        public IActionResult Create(CreateEventViewModel model, string ticketTypesData)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with validation errors
            }

            // Create the event
            var newEvent = new Event
            {
                Name = model.EventName,
                Location = model.EventLocation,
                Description = model.EventDescription,
                StartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartTime.Hours, model.StartTime.Minutes, 0),
                EndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndTime.Hours, model.EndTime.Minutes, 0),
                CreatorId = _session.GetCurrentUser().Id
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges(); // Save the event to get the ID

            // Deserialize the ticket types data
            var ticketTypes = JsonConvert.DeserializeObject<List<TicketType>>(ticketTypesData);

            // Associate ticket types with the event
            foreach (var ticketType in ticketTypes)
            {
                ticketType.EventId = newEvent.Id; // Assuming TicketType has an EventId property
                _context.TicketTypes.Add(ticketType);
            }

            _context.SaveChanges(); // Save the ticket types

            return RedirectToAction("Index", "Events"); // Redirect to the events list or another appropriate action
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

            var model = new OrganizerEventViewModel
            {
                EventId = eventId,
                EventName = eventDetails?.Name ?? String.Empty,
                EventLocation = eventDetails?.Location ?? String.Empty,
                EventDescription = eventDetails?.Description ?? String.Empty,
                TicketTypes = ticketTypes,
                TicketsSold = _ticketService.GetEventTicketsSold(eventId),
                Revenue = _eventService.GetEventRevenue(eventId),
                StartDate = eventDetails.StartTime?.Date ?? DateTime.Now.Date,
                StartTime = eventDetails.StartTime?.TimeOfDay ?? TimeSpan.Zero, // Use TimeOfDay to get TimeSpan
                EndDate = eventDetails.EndTime?.Date ?? DateTime.Now.Date,
                EndTime = eventDetails.EndTime?.TimeOfDay ?? TimeSpan.Zero // Use TimeOfDay to get TimeSpan
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
                TicketsSold = 0,
                Revenue = 0,
                StartDate = eventDetails.StartTime?.Date ?? DateTime.Now.Date,
                StartTime = eventDetails.StartTime ?? DateTime.Now,
                EndDate = eventDetails.EndTime?.Date ?? DateTime.Now.Date,
                EndTime = eventDetails.EndTime ?? DateTime.Now
            };

            return View("UserView", model);
        }

        [HttpPost]
        public IActionResult Save(OrganizerEventViewModel model, string ticketTypesData)
        {
            var eventToUpdate = _context.Events.Find(model.EventId);
            if (eventToUpdate != null)
            {
                eventToUpdate.Name = model.EventName;
                eventToUpdate.Location = model.EventLocation;
                eventToUpdate.Description = model.EventDescription;
                eventToUpdate.StartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartTime.Hours, model.StartTime.Minutes, 0);
                eventToUpdate.EndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndTime.Hours, model.EndTime.Minutes, 0);

                // Parse the ticket types data
                var ticketTypes = JsonConvert.DeserializeObject<List<TicketType>>(ticketTypesData);

                // Update or create ticket types
                foreach (var ticket in ticketTypes)
                {
                    var existingTicket = _context.TicketTypes.Find(ticket.Id);
                    if (existingTicket != null)
                    {
                        // Update existing ticket type
                        existingTicket.Name = ticket.Name;
                        existingTicket.Price = ticket.Price;
                        existingTicket.Quantity = ticket.Quantity;
                    }
                    else
                    {
                        // Create new ticket type
                        _context.TicketTypes.Add(new TicketType
                        {
                            Name = ticket.Name,
                            Price = ticket.Price,
                            Quantity = ticket.Quantity,
                            EventId = model.EventId // Associate with the event
                        });
                    }
                }

                _context.SaveChanges(); // Save all changes to the database
            }

            return RedirectToAction("Index", "Events"); // Redirect to the event list or another appropriate action
        }
    }
}