namespace Eventure_ASP.Models
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Event> Events { get; set; } = new List<Event>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<Event> EventHistory { get; set; } = new List<Event>();
        public List<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        // Properties for the selected payment method
        public int SelectedPaymentMethodId { get; set; } // To hold the ID of the selected payment method
        public string AccountNumber { get; set; } = string.Empty; // To hold the account number
        public string ExpirationDate { get; set; } = string.Empty; // To hold the expiration date in MM/yyyy format
        public bool IsDefault { get; set; } = false; // To indicate if the payment method is default
    }
}