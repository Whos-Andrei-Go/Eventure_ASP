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
    }
}