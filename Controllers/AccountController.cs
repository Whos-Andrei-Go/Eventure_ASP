using Eventure_ASP.Models;
using Eventure_ASP.Data;
using Eventure_ASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net; // Make sure to install the BCrypt.Net NuGet package
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly Session _session;
    private readonly UserRepository _userRepository;

    public AccountController(Session session, UserRepository userRepository)
    {
        _session = session;
        _userRepository = userRepository;
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Validate the user credentials
            var user = await ValidateUser(model.Username, model.Password);
            if (user != null)
            {
                _session.SetCurrentUser(user); // Set the current user in session
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
        }
        return View(model);
    }

    private async Task<User> ValidateUser(string username, string password)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) // Assuming PasswordHash is the hashed password in the database
        {
            return user; // Return the user if the password matches
        }
        return null; // Return null if the user is not found or password does not match
    }
}