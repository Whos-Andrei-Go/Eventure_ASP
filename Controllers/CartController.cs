﻿using Microsoft.AspNetCore.Mvc;
using Eventure_ASP.Models;
using Eventure_ASP.Services;

namespace Eventure_ASP.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly TicketService _ticketService;

        public CartController(CartService cartService, TicketService ticketService)
        {
            _cartService = cartService;
            _ticketService = ticketService;
        }

        [HttpPost]
        public IActionResult AddToCart(int ticketTypeId, int quantity, int eventId)
        {
            var ticketType = _ticketService.GetTicketTypeById(ticketTypeId);
            if (ticketType != null && quantity > 0)
            {
                _cartService.AddTicket(ticketType, quantity);
                TempData["SuccessMessage"] = "Ticket(s) added to cart successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add ticket to cart.";
            }

            // Redirect back to the event view using the eventId
            return RedirectToAction("UserView", "Event", new { eventId = eventId });
        }
    }
}