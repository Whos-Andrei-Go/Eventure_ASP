using Eventure_ASP.Data;
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
    }
}
