using System.Collections.Generic;
using System;

namespace Eventure_ASP.Models
{
    public class CheckoutViewModel
    {
        public List<CartTicket> CartTickets { get; set; } = new List<CartTicket>();
        public decimal CartTotal { get; set; }
    }
}