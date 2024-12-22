using Eventure_ASP.Utilities;
using Eventure_ASP.Models;
using Eventure_ASP.Services;
using Microsoft.AspNetCore.Mvc;
using Eventure_ASP.Data;
using System.Globalization;

namespace Eventure_ASP.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CartService _cartService;
        private readonly EtsDbContext _context; // Your DbContext for accessing the database
        private readonly PaymentMethodService _paymentMethodService; // Injecting PaymentMethodService
        private readonly Session _session; // Assuming you have a SessionService for session management

        public CheckoutController(CartService cartService, EtsDbContext context, PaymentMethodService paymentMethodService, Session session)
        {
            _cartService = cartService;
            _context = context; // Initialize the DbContext
            _paymentMethodService = paymentMethodService; // Initialize the PaymentMethodService
            _session = session; // Initialize the SessionService
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

        public IActionResult EnterPaymentMethod()
        {
            var currentUserId = _session.GetCurrentUser().Id;

            var model = new EnterPaymentMethodViewModel
            {
                PaymentMethods = _paymentMethodService.GetPaymentMethodsByUserId(currentUserId),
                CartTickets = _cartService.GetCartTickets(),
                CartTotal = _cartService.GetCartTotal()
            };

            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult CompleteCheckout(EnterPaymentMethodViewModel model)
        {
            var currentUserId = _session.GetCurrentUser().Id; // Get the current user's ID from the session

            if (ModelState.IsValid)
            {
                // Retrieve the selected payment method from the database using PaymentMethodId
                var paymentMethod = _paymentMethodService.GetPaymentMethodById(model.PaymentMethodId);

                if (paymentMethod == null)
                {
                    ModelState.AddModelError("", "Selected payment method not found.");
                    return View("EnterPaymentMethod", model);
                }

                // Process the payment and add tickets
                bool paymentSuccess = _paymentMethodService.ProcessPayment(paymentMethod, model.CartTickets);

                if (paymentSuccess)
                {
                    _cartService.ClearCart();

                    // Set a success message
                    TempData["SuccessMessage"] = "Your payment was successful! Thank you for your purchase.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There was an issue processing your payment. Please try again.");
                }
            }

            // If model state is invalid or payment failed, return the same view with the model to show errors
            
            model.PaymentMethods = _paymentMethodService.GetPaymentMethodsByUserId(currentUserId);
            model.CartTickets = _cartService.GetCartTickets();
            model.CartTotal = _cartService.GetCartTotal();
            return View("EnterPaymentMethod", model);
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
