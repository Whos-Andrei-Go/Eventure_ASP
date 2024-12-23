using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Eventure_ASP.Controllers
{
    public class UsersController : Controller
    {
        private readonly EtsDbContext _context;
        private readonly Session _session;

        public UsersController(EtsDbContext context, Session session) // Inject the Session class
        {
            _context = context;
            _session = session; // Initialize the session field

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Manage()
        {
            var users = _context.Users
                .ToList();

            return View(users);
        }

        [HttpPost]
        public IActionResult UpdateUser(User model)
        {
            // Find the user in the database by ID
            var user = _context.Users.Find(model.Id);

            if (user != null)
            {
                // Update user properties
                user.Username = model.Username;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Role = model.Role;
                user.DateUpdated = DateTime.Now;

                _context.SaveChanges();

                return Json(new { success = true, message = "User updated successfully." });
            }

            // If user not found, return an error response
            return Json(new { success = false, message = "User  not found." });
        }
    }
}
