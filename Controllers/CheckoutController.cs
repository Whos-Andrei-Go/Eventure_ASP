using Eventure_ASP.Utilities;
using Eventure_ASP.Models;
using Eventure_ASP.Services;
using Microsoft.AspNetCore.Mvc;
using Eventure_ASP.Data;
using ZXing.SkiaSharp;
using System.Drawing.Imaging;
using System.IO;
using ZXing.QrCode;
using System.Drawing;
using ZXing;
using SkiaSharp;

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

            var paymentMethods = _paymentMethodService.GetPaymentMethodsByUserId(currentUserId);

            // Find the default payment method ID
            var defaultPaymentMethodId = paymentMethods.FirstOrDefault(pm => pm.IsDefault)?.Id;

            var model = new EnterPaymentMethodViewModel
            {
                PaymentMethods = paymentMethods,
                CartTickets = _cartService.GetCartTickets(),
                CartTotal = _cartService.GetCartTotal(),
                DefaultPaymentMethodId = defaultPaymentMethodId 
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteCheckout(EnterPaymentMethodViewModel model)
        {
            var currentUserId = _session.GetCurrentUser().Id; // Get the current user's ID from the session
            model.CartTickets = _cartService.GetCartTickets();

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
                // Create tickets for each ticket type in the cart
                foreach (var cartTicket in model.CartTickets)
                {
                    var ticketType = _context.TicketTypes.Find(cartTicket.TicketType.Id);
                    if (ticketType == null)
                    {
                        // Handle the case where the ticket type is not found
                        ModelState.AddModelError("", "Ticket type not found.");
                        return View("EnterPaymentMethod", model);
                    }

                    // Deduct the quantity from the TicketType
                    ticketType.Quantity -= cartTicket.Quantity; // Assuming AvailableQuantity is the property to track available tickets

                    for (int i = 0; i < cartTicket.Quantity; i++)
                    {
                        var ticket = new Ticket
                        {
                            UserId = currentUserId,
                            TicketTypeId = cartTicket.TicketType.Id,
                            PurchaseDate = DateTime.Now,
                            Status = "Active", // Set the status as needed
                            QrCode = GenerateQrCode($"User Id:{currentUserId}, TicketTypeId:{cartTicket.TicketType.Id}, PurchaseDate:{DateTime.Now}") // Generate QR code with relevant data
                        };

                        // Save the ticket to the database
                        _context.Tickets.Add(ticket);
                    }
                }

                _context.SaveChanges();

                _cartService.ClearCart();

                // Set a success message
                TempData["SuccessMessage"] = "Your payment was successful! Thank you for your purchase.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "There was an issue processing your payment. Please try again.";
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
                TempData["ErrorMessage"] = "Ticket type not found.";
                return RedirectToAction("Index"); // Redirect to an appropriate page, e.g., the ticket listing page
            }

            // Add the ticket type to the cart
            _cartService.AddTicket(ticketType, quantity);

            // Redirect to the cart or another page
            return RedirectToAction("Index"); // Redirect to the cart view or another appropriate action
        }

        private string GenerateQrCode(string data)
        {
            // Create a QR code writer
            var qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 1 // Optional: Set margin around the QR code
                }
            };

            // Generate the QR code as a bitmap
            using (var bitmap = qrWriter.Write(data))
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Save the bitmap to the memory stream in PNG format
                    bitmap.Encode(memoryStream, SKEncodedImageFormat.Png, 100);
                    // Convert the bitmap to a base64 string
                    var qrCodeImage = Convert.ToBase64String(memoryStream.ToArray());
                    // Return the base64 string prefixed with the appropriate data URI scheme
                    return $"data:image/png;base64,{qrCodeImage}";
                }
            }
        }
    }
}
