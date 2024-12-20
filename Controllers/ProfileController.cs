using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Eventure_ASP.Controllers
{
    public class ProfileController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session; // Inject the Session utility
        private readonly PasswordUtils _passwordUtils; // Inject the Password utility

        public ProfileController(EtsDbContext context, Session session, PasswordUtils passwordUtils)
        {
            _context = context;
            _session = session; // Assign the injected session utility
            _passwordUtils = passwordUtils; // Assign the injected password utility
        }

        private ProfileViewModel getModel()
        {
            var currentDate = DateTime.Now;

            // Assuming you have a way to get the user ID from the session
            var userId = _session.GetCurrentUser().Id;

            // Fetch the user from the database
            var user = _context.Users.Find(userId); // Adjust this based on your user entity

            var model = new ProfileViewModel
            {
                FirstName = user?.FirstName,
                LastName = user?.LastName,
                Username = user?.Username,
                Email = user?.Email,
                Events = _context.Events.ToList(),
                Tickets = _context.Tickets
                    .Include(t => t.TicketType) // Eager load TicketType
                    .ToList(),
                EventHistory = _context.Events
                    .Where(e => e.StartTime.HasValue && e.StartTime.Value < currentDate)
                    .ToList(),
                PaymentMethods = null
            };

            return model;
        }

        public IActionResult Index()
        {
            return View(getModel());
        }

        public IActionResult LoadView(string view)
        {
            var model = getModel();

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

        [HttpPost]
        public IActionResult UpdateUser(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _session.GetCurrentUser().Id; // Get the user ID from the session
                var user = _context.Users.Find(userId);

                if (user != null)
                {
                    // Update user details
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Username = model.Username;
                    user.Email = model.Email;

                    // Check if the current password is provided
                    if (!string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        // Verify the current password
                        if (_passwordUtils.VerifyPassword(user.Password, model.CurrentPassword))
                        {
                            // Update the password if a new one is provided
                            if (!string.IsNullOrEmpty(model.NewPassword))
                            {
                                user.Password = _passwordUtils.HashPassword(model.NewPassword); // Hash the new password before saving
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                            return View(getModel()); // Return the view with the model to show the error
                        }
                    }

                    _context.SaveChanges(); // Save changes to the database
                    return RedirectToAction("Overview"); // Redirect to the overview page after successful update
                }
            }

            return View(getModel()); // Return the view with the model if validation fails
        }
    }
}