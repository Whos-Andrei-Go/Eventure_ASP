namespace Eventure_ASP.Models
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime JoinedDate { get; set; }
        public List<Event> Events { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Event> EventHistory { get; set; }
        public List<PaymentMethodModel> PaymentMethods { get; set; }
    }

    public class PaymentMethodModel
    {
        public string Type { get; set; }
        public string LastFourDigits { get; set; }
        public string Status { get; set; }
    }
}