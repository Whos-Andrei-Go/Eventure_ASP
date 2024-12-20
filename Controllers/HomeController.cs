using System.Diagnostics;
using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eventure_ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EtsDbContext _context;

        public HomeController(ILogger<HomeController> logger, EtsDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
