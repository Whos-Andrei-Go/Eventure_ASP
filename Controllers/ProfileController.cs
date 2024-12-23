using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventure_ASP.Controllers
{
    public class ProfileController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session;
        private readonly PasswordUtils _passwordUtils;

        public ProfileController(EtsDbContext context, Session session, PasswordUtils passwordUtils)
        {
            _context = context;
            _session = session;
            _passwordUtils = passwordUtils;
        }

        private ProfileViewModel GetModel()
        {
            var currentDate = DateTime.Now;
            var user = _session.GetCurrentUser();

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

            if (user != null)
            {
                model.FirstName = user.FirstName ?? string.Empty;
                model.LastName = user.LastName ?? string.Empty;
                model.Username = user.Username ?? string.Empty;
                model.Email = user.Email;
                model.Events = _context.Events.ToList();
                model.Tickets = _context.Tickets.Include(t => t.TicketType).ToList();
                model.EventHistory = _context.Events
                    .Include(e => e.Creator)
                    .Where(e => e.StartTime.HasValue && e.StartTime.Value < currentDate)
                    .ToList();
                model.PaymentMethods = _context.PaymentMethods
                    .Where(pm => pm.UserId == user.Id)
                    .ToList();
            }

            return model;
        }

        public IActionResult Index()
        {
            var model = GetModel();
            return View(model);
        }

        public IActionResult Admin()
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
            var user = _session.GetCurrentUser();

            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.Username;
                user.Email = model.Email;

                if (!string.IsNullOrEmpty(model.CurrentPassword))
                {
                    if (_passwordUtils.VerifyPassword(user.Password, model.CurrentPassword))
                    {
                        if (!string.IsNullOrEmpty(model.NewPassword))
                        {
                            user.Password = _passwordUtils.HashPassword(model.NewPassword);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                        return View("Index", GetModel());
                    }
                }

                _context.SaveChanges();
                return View("Index", GetModel());
            }

            return View("Index", GetModel());
        }

        public IActionResult DeleteAccount()
        {
            var userId = _session.GetCurrentUser().Id;
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                _session.Logout();
                return RedirectToAction("Login", "Account");
            }
            return NotFound();
        }

        public IActionResult Logout()
        {
            _session.Logout();
            return RedirectToAction("Login", "Account");
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
        public IActionResult AddPaymentMethod(string paymentType, string accountNumber, string expirationDate)
        {
            var userId = _session.GetCurrentUser().Id; // Get the current user's ID

            // Create a new PaymentMethod object
            var paymentMethod = new PaymentMethod
            {
                UserId = userId,
                PaymentType = paymentType,
                AccountNumber = accountNumber,
                ExpirationDate = DateTime.TryParseExact(expirationDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expDate) ? DateOnly.FromDateTime(expDate) : (DateOnly?)null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the payment method to the database
            _context.PaymentMethods.Add(paymentMethod);
            _context.SaveChanges();

            // Optionally, return the updated list of payment methods or a success message
            return Json(new { success = true, message = "Payment method added successfully." });
        }

        [HttpPost]
        public IActionResult UpdatePaymentMethod(int selectedPaymentMethodId, ProfileViewModel model)
        {
            var paymentMethod = _context.PaymentMethods.Find(selectedPaymentMethodId);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            // Check if the user is trying to set this payment method as default
            if (model.IsDefault)
            {
                // Check if another payment method is already set as default
                var existingDefault = _context.PaymentMethods
                    .FirstOrDefault(pm => pm.UserId == paymentMethod.UserId && pm.IsDefault && pm.Id != selectedPaymentMethodId);

                if (existingDefault != null)
                {
                    TempData["ErrorMessage"] = "Another payment method is already set as default. Please unset it before making this one default.";
                    TempData["PartialView"] = "_PaymentMethods";
                    return View("Index", GetModel());
                }
            }

            // Proceed with the update
            paymentMethod.AccountNumber = model.AccountNumber;
            paymentMethod.IsDefault = model.IsDefault;

            if (DateTime.TryParseExact(model.ExpirationDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expirationDate))
            {
                // Convert DateTime to DateOnly before assigning
                paymentMethod.ExpirationDate = DateOnly.FromDateTime(expirationDate);
            }
            else
            {
                paymentMethod.ExpirationDate = null; // Handle invalid date
            }

            _context.SaveChanges();

            TempData["PartialView"] = "_PaymentMethods";
            return View("Index", GetModel());
        }

        [HttpPost]
        public IActionResult DeletePaymentMethod(int id)
        {
            var paymentMethod = _context.PaymentMethods.Find(id);
            if (paymentMethod == null)
            {
                return Json(new { success = false, message = "Payment method does not exist." });
            }

            _context.PaymentMethods.Remove(paymentMethod);
            _context.SaveChanges();

            return Json(new { success = true, message = "Payment method deleted successfully." });
        }
    }
}