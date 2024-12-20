using Eventure_ASP.Models;
using Eventure_ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eventure_ASP.Utilities; // Include the namespace for the Session class

namespace Eventure_ASP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session; // Add a field for the Session class

        public DashboardController(EtsDbContext context, Session session) // Inject the Session class
        {
            _context = context;
            _session = session; // Initialize the session field
        }

        public IActionResult Index()
        {
            var upcomingEvents = GetUpcomingEvents() ?? new List<Event>(); // Ensure no null
            var userEvents = GetUserEvents() ?? new List<Event>(); // Ensure no null

            var viewModel = new DashboardViewModel
            {
                UpcomingEvents = upcomingEvents,
                YourEvents = userEvents
            };

            return View(viewModel);
        }

        private List<Event> GetUpcomingEvents()
        {
            return _context.Events
                .Include(e => e.Creator) // Ensure Creator data is loaded
                .Where(e => e.StartTime > DateTime.Now) // Filter for upcoming events
                .OrderBy(e => e.StartTime)
                .Take(5) // Limit to 5 upcoming events
                .ToList();
        }

        private List<Event> GetUserEvents()
        {
            var currentUser = _session.GetCurrentUser(); // Get the current user from the session

            if (currentUser == null)
            {
                return new List<Event>(); // Return an empty list if no user is found
            }

            // Assuming you have a UserId property in your User model
            return _context.Events
                .Include(e => e.Creator) // Ensure Creator data is loaded
                .Where(e => e.CreatorId == currentUser.Id) // Filter events by the current user's ID
                .OrderBy(e => e.StartTime)
                .Take(5) // Limit to 5 of the user's upcoming events
                .ToList();
        }
    }
}