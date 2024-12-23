using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Services;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventure_ASP.Controllers
{
    public class EventsController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session;
        private readonly EventService _eventService;
        private readonly TicketService _ticketService;

        public EventsController(EtsDbContext context, Session session, EventService eventService, TicketService ticketService) // Inject the Session class and EventService
        {
            _context = context;
            _session = session; // Initialize the session field
            _eventService = eventService; // Initialize the EventService field
            _ticketService = ticketService; // Initialize the EventService field
        }

        public IActionResult Index()
        {
            var events = _context.Events
                .Include(e => e.Creator)
                .ToList();

            return View(events);
        }

        public IActionResult Manage()
        {
            var events = _context.Events
                .Include(e => e.Creator)
                .ToList();

            var manageEventViewModels = events.Select(e => new ManageEventsViewModel
            {
                Id = e.Id,
                CreatorId = e.CreatorId,
                Name = e.Name,
                Description = e.Description,
                Location = e.Location,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                TotalTicketsSold = _context.Tickets
                    .Count(t => t.TicketType.EventId == e.Id) // Calculate total tickets sold
            }).ToList();

            return View(manageEventViewModels);
        }

        public IActionResult EditEvent(int id)
        {
            var eventItem = _eventService.GetEventById(id); // Use the EventService to get the event by ID
            if (eventItem == null)
            {
                return NotFound();
            }

            var viewModel = new EventViewModel
            {
                Event = eventItem,
                TicketTypes = _ticketService.GetTicketTypesByEventId(id) // Use the EventService to get ticket types by event ID
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateEvent(Event eventItem)
        {
            if (ModelState.IsValid)
            {
                _eventService.UpdateEvent(eventItem); // Use the EventService to update the event
                return RedirectToAction("Manage");
            }

            return View(eventItem);
        }
    }
}