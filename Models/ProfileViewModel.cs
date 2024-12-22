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
    }
}