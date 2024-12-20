using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventure_ASP.Controllers
{
    public class ProfileController : Controller
    {
        private readonly EtsDbContext _context;

        public ProfileController(EtsDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadView(string view)
        {
            var currentDate = DateTime.Now;

            var model = new ProfileViewModel
            {
                Events = _context.Events.ToList(),
                Tickets = _context.Tickets.ToList(),
                EventHistory = _context.Events
                   .Where(e => e.StartTime.HasValue && e.StartTime.Value < currentDate)
                   .ToList(),
                PaymentMethods = null
            };

            switch (view)
            {
                case "Overview":
                    return PartialView("_Overview", model);
                case "MyEvents":
                    return PartialView("_MyEvents", model);
                case "MyTickets":
                    return PartialView("_MyTickets", model);
                case "EventHistory":
                    return PartialView("_EventHistory", model);
                case "PaymentMethods":
                    return PartialView("_PaymentMethods", model);
                default:
                    return PartialView("_Overview", model);
            }
        }
    }
}