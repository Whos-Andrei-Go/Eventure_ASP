using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models
{
    public class EnterPaymentMethodViewModel
    {
        public List<PaymentMethod> PaymentMethods { get; set; }
        public List<CartTicket> CartTickets { get; set; }
        public decimal CartTotal { get; set; }

        // New properties for payment method details
        public int PaymentMethodId { get; set; } // Assuming PaymentMethodId is an integer
        public string AccountNumber { get; set; }
        public string ExpirationDate { get; set; } // Keep this as a string for input purposes
    }
}