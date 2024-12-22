using Eventure_ASP.Data;
using Eventure_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventure_ASP.Services
{
    public class PaymentMethodService
    {
        private readonly EtsDbContext _context; // Assuming you're using Entity Framework

        public PaymentMethodService(EtsDbContext context)
        {
            _context = context;
        }

        // Get payment methods for a specific user
        public List<PaymentMethod> GetPaymentMethodsByUserId(int userId)
        {
            return _context.PaymentMethods
                .Where(pm => pm.UserId == userId)
                .ToList();
        }

        public PaymentMethod GetPaymentMethodById(int paymentMethodId)
        {
            return _context.PaymentMethods.Find(paymentMethodId); // Assuming PaymentMethods is your DbSet
        }

        public void AddPaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            _context.PaymentMethods.Add(paymentMethod);
            _context.SaveChanges(); // Save changes to the database
        }

        public bool ProcessPayment(PaymentMethod paymentMethod, List<CartTicket> cartTickets)
        {
            // Here you would implement the logic to process the payment
            // For example, you might integrate with a payment gateway API

            // For demonstration purposes, let's assume the payment is always successful
            // You would replace this with actual payment processing logic
            return true;
        }
    }
}