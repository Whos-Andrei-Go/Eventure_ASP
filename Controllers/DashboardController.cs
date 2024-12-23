using Eventure_ASP.Models;
using Eventure_ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eventure_ASP.Utilities;
using Eventure_ASP.Services;
using System.Linq; // Ensure you have this for LINQ methods

namespace Eventure_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session; // Add a field for the Session class
        private readonly EventService _eventService;
        private readonly TicketService _ticketService;

        public DashboardController(EtsDbContext context, Session session, EventService eventService, TicketService ticketService) // Inject the Session class
        {
            _context = context;
            _session = session;

            _eventService = eventService;
            _ticketService = ticketService;
        }

        public IActionResult Index()
        {
            var upcomingEvents = GetUserUpcomingEvents() ?? new List<Event>(); // Ensure no null
            var userEvents = GetUserEvents() ?? new List<Event>(); // Ensure no null
            var userRole = _session.GetCurrentUser()?.Role ?? "User"; // Set the role based on the user's claims

            var viewModel = new DashboardViewModel
            {
                UpcomingEvents = upcomingEvents,
                YourEvents = userEvents,
                UserRole = userRole
            };

            return View(viewModel);
        }

        public IActionResult Admin()
        {
            var upcomingEvents = GetUpcomingEvents() ?? new List<Event>(); // Ensure no null
            var totalTicketsSold = _ticketService.GetTotalTicketsSold();
            var totalRevenue = _ticketService.GetTotalRevenue();
            var upcomingEventsCount = upcomingEvents.Count; // Count of upcoming events
            var topSellingEventName = _eventService.GetTopSellingEventName();
            var totalRegisteredUsers = _context.Users.Count();
            var userRole = _session.GetCurrentUser()?.Role ?? "User "; // Set the role based on the user's claims

            var viewModel = new AdminDashboardViewModel
            {
                TotalTicketsSold = totalTicketsSold,
                TotalRevenue = totalRevenue,
                UpcomingEventsCount = upcomingEventsCount,
                TopSellingEventName = topSellingEventName,
                TotalRegisteredUsers = totalRegisteredUsers,
            };

            return View(viewModel);
        }

        private List<Event> GetUpcomingEvents()
        {
            // Get the user's tickets and join with TicketType to get EventId
            var daUpcomingEvents = _context.Tickets
                .Include(t => t.TicketType) // Include TicketType to access EventId
                .Select(t => t.TicketType.EventId) // Select EventId from TicketType
                .Distinct() // Ensure unique EventIds
                .ToList();

            // Get upcoming events based on the user's tickets
            var upcomingEvents = _context.Events
                .Include(e => e.Creator) // Ensure Creator data is loaded
                .Where(e => daUpcomingEvents.Contains(e.Id) && e.StartTime > DateTime.Now) // Filter for events with matching EventId and future start time
                .OrderBy(e => e.StartTime)
                .ToList();

            return upcomingEvents;
        }

        private List<Event> GetUserUpcomingEvents()
        {
            var currentUser = _session.GetCurrentUser(); // Get the current user from the session

            if (currentUser == null)
            {
                return new List<Event>(); // Return an empty list if no user is found
            }

            // Get the user's tickets and join with TicketType to get EventId
            var userUpcomingEvents = _context.Tickets
                .Where(t => t.UserId == currentUser.Id) // Filter tickets by the current user's ID
                .Include(t => t.TicketType) // Include TicketType to access EventId
                .Select(t => t.TicketType.EventId) // Select EventId from TicketType
                .Distinct() // Ensure unique EventIds
                .ToList();

            // Get upcoming events based on the user's tickets
            var upcomingEvents = _context.Events
                .Include(e => e.Creator) // Ensure Creator data is loaded
                .Where(e => userUpcomingEvents.Contains(e.Id) && e.StartTime > DateTime.Now) // Filter for events with matching EventId and future start time
                .OrderBy(e => e.StartTime)
                .ToList();

            return upcomingEvents;
        }

        private List<Event> GetUserEvents()
        {
            var currentUser = _session.GetCurrentUser(); // Get the current user from the session

            if (currentUser == null)
            {
                return new List<Event>(); // Return an empty list if no user is found
            }

            // Get events for the tickets that are upcoming
            return _context.Events
                .Include(e => e.Creator) // Ensure Creator data is loaded
                .Where(e => e.CreatorId == currentUser.Id) // Filter events by the current user's ID
                .OrderBy(e => e.StartTime)
                .Take(5) // Limit to 5 of the user's upcoming events
                .ToList();
        }
    }
}