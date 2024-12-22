using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        private ProfileViewModel GetModel()
        {
            var currentDate = DateTime.Now;

            var userId = _session.GetCurrentUser()?.Id;

            var model = new ProfileViewModel
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Username = string.Empty,
                Email = string.Empty,
                Events = new List<Event>(),
                Tickets = new List<Ticket>(),
                EventHistory = new List<Event>(),
                PaymentMethods = new List<PaymentMethod>(),
                CurrentPassword = string.Empty,
                NewPassword = string.Empty
            };

            if (userId == null)
            {
                return model;
            }

            var user = _context.Users.Find(userId);

            if (user != null)
            {
                model.FirstName = user.FirstName ?? String.Empty;
                model.LastName = user.LastName ?? String.Empty;
                model.Username = user.Username ?? String.Empty;
                model.Email = user.Email;
                model.Events = _context.Events.ToList();
                model.Tickets = _context.Tickets.Include(t => t.TicketType).ToList();
                model.EventHistory = _context.Events
                    .Where(e => e.StartTime.HasValue && e.StartTime.Value < currentDate)
                    .ToList();
                model.PaymentMethods = _context.PaymentMethods
                    .Where(pm => pm.UserId == userId)
                    .ToList();
            }

            return model;
        }

        public IActionResult Index()
        {
            return View(GetModel());
        }

        public IActionResult LoadView(string view)
        {
            var model = GetModel();

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
            var user = _session.GetCurrentUser(); // Get the user ID from the session

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
                        return View("Index", GetModel()); // Return to the main profile view
                    }
                }

                _context.SaveChanges(); // Save changes to the database
                return View("Index", GetModel()); // Return to the main profile view
            }

            return View("Index", GetModel()); // Return to the main profile view if validation fails
        }

        public IActionResult DeleteAccount()
        {
            var userId = _session.GetCurrentUser().Id; // Assuming you have a method to get the current user's ID
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                _session.Logout();
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }
            return NotFound(); // Handle user not found case
        }

        public IActionResult Logout()
        {
            _session.Logout(); // Clear session or perform logout logic
            return RedirectToAction("Login", "Account"); // Redirect to login page
        }

        [HttpGet]
        public IActionResult GetPaymentMethod(int id)
        {
            var paymentMethod = _context.PaymentMethods.Find(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            return Json(new
            {
                accountNumber = paymentMethod.AccountNumber,
                expirationDate = paymentMethod.ExpirationDate?.ToString("MM/yyyy"),
                isDefault = paymentMethod.IsDefault
            });
        }

        [HttpPost]
        public IActionResult UpdatePaymentMethod(ProfileViewModel model, int id)
        {
            var paymentMethod = model.PaymentMethods.FirstOrDefault(pm => pm.Id == id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            paymentMethod.AccountNumber = model.PaymentMethods.FirstOrDefault(pm => pm.Id == id)?.AccountNumber ?? string.Empty;
            paymentMethod.IsDefault = model.PaymentMethods.FirstOrDefault(pm => pm.Id == id)?.IsDefault ?? false;

            var expirationDateStr = model.PaymentMethods.FirstOrDefault(pm => pm.Id == id)?.ExpirationDate;

            if (expirationDateStr.HasValue)
            {
                // Convert DateOnly to string in "MM/yyyy" format
                paymentMethod.ExpirationDate = expirationDateStr.Value; // Assign the DateOnly value directly
            }
            else
            {
                paymentMethod.ExpirationDate = null; // Clear if invalid
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Payment method updated successfully." });
        }

        [HttpPost]
        public IActionResult DeletePaymentMethod(int id)
        {
            var paymentMethod = _context.PaymentMethods.Find(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            _context.PaymentMethods.Remove(paymentMethod);
            _context.SaveChanges();
            return Json(new { success = true, message = "Payment method deleted successfully." });
        }
    }
}