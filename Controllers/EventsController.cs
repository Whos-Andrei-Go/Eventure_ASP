using Eventure_ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventure_ASP.Controllers
{
    public class EventsController : Controller
    {
        private readonly EtsDbContext _context;

        public EventsController(EtsDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var events = _context.Events
                .Include(e => e.Creator) // Assuming Creator is a navigation property
                .ToList();

            return View(events);
        }
    }
}
