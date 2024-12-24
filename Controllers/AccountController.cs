using Eventure_ASP.Models;
using Eventure_ASP.Data;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net; // Make sure to install the BCrypt.Net NuGet package
using System.Linq; // For LINQ methods

namespace Eventure_ASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly Session _session;
        private readonly UserRepository _userRepository;
        private readonly EtsDbContext _context; // Use EtsDbContext

        public AccountController(Session session, UserRepository userRepository, EtsDbContext context)
        {
            _session = session;
            _userRepository = userRepository;
            _context = context; // Initialize the EtsDbContext
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validate the user credentials
                var user = ValidateUser(model.Username, model.Password);
                if (user != null)
                {
                    _session.SetCurrentUser(user);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Admin", "Dashboard");
                    }

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already taken.");
                    return View(model);
                }

                // Create a new user
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash the password
                    Role = "User"
                };

                _context.Users.Add(user);
                _context.SaveChanges(); // Save changes synchronously

                var newUser = _userRepository.GetUserByUsername(model.Username);
                _session.SetCurrentUser(newUser); // Set the current user in session


                // Redirect to login after successful registration
                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }

        private User ValidateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) // Verify the hashed password
            {
                return user; // Return the user if the password matches
            }
            return null; // Return null if the user is not found or password does not match
        }
    }
}