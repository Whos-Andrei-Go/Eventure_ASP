using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Eventure_ASP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventure_ASP.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CartService _cartService;
        private readonly EtsDbContext _context; // Your DbContext for accessing the database

        public CheckoutController(CartService cartService, EtsDbContext context)
        {
            _cartService = cartService;
            _context = context; // Initialize the DbContext
        }

        public IActionResult Index()
        {
            var model = new CheckoutViewModel
            {
                CartTickets = _cartService.GetCartTickets(),
                CartTotal = _cartService.GetCartTotal()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteCheckout(string currentPassword, string newPassword)
        {
            // Handle checkout logic here (e.g., validate password, process payment, etc.)
            // Clear the cart after successful checkout
            _cartService.ClearCart();
            return RedirectToAction("Index", "Home"); // Redirect to a confirmation page or home
        }

        [HttpPost] // Use HttpPost if this action is triggered by a form submission
        public IActionResult AddToCart(int ticketTypeId, int quantity)
        {
            // Retrieve the TicketType from the database using ticketTypeId
            var ticketType = _context.TicketTypes.Find(ticketTypeId); // Assuming TicketTypes is your DbSet

            if (ticketType == null)
            {
                // Handle the case where the ticket type is not found
                ModelState.AddModelError("", "Ticket type not found.");
                return RedirectToAction("Index"); // Redirect to an appropriate page, e.g., the ticket listing page
            }

            // Add the ticket type to the cart
            _cartService.AddTicket(ticketType, quantity);

            // Redirect to the cart or another page
            return RedirectToAction("Index"); // Redirect to the cart view or another appropriate action
        }
    }
}
