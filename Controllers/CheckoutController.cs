using Eventure_ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventure_ASP.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly EtsDbContext _context;

        public CheckoutController(EtsDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadView(string view)
        {
            switch (view)
            {
                case "Overview":
                    return PartialView("_Overview");
                case "MyEvents":
                    return PartialView("_MyEvents");
                case "MyTickets":
                    return PartialView("_MyTickets");
                case "EventHistory":
                    return PartialView("_EventHistory");
                case "PaymentMethods":
                    return PartialView("_PaymentMethods");
                default:
                    return PartialView("_Overview");
            }
        }
    }
}
