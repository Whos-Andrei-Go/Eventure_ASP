using Eventure_ASP.Models;
using System.Collections.Generic;
using System.Linq;


namespace Eventure_ASP.Services
{
    public class CartService
    {
        private readonly List<CartTicket> _cartTickets = new List<CartTicket>();

        public void AddTicket(TicketType ticketType, int quantity)
        {
            var existingTicket = _cartTickets.FirstOrDefault(t => t.TicketType.Id == ticketType.Id);
            if (existingTicket != null)
            {
                existingTicket.Quantity += quantity;
            }
            else
            {
                _cartTickets.Add(new CartTicket { TicketType = ticketType, Quantity = quantity });
            }
        }

        public void RemoveTicket(int ticketTypeId)
        {
            var ticket = _cartTickets.FirstOrDefault(t => t.TicketType.Id == ticketTypeId);
            if (ticket != null)
            {
                _cartTickets.Remove(ticket);
            }
        }

        public List<CartTicket> GetCartTickets()
        {
            return _cartTickets;
        }

        public decimal GetCartTotal()
        {
            return _cartTickets.Sum(t => t.TicketType.Price * t.Quantity);
        }

        public void ClearCart()
        {
            _cartTickets.Clear();
        }
    }
}
