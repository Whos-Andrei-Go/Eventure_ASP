using System.Collections.Generic;

namespace Eventure_ASP.Models
{
    public class OrganizerEventViewModel
    {
        public Event Event { get; set; }
        public List<TicketType> TicketTypes { get; set; }
        public string TicketsSold { get; set; }
        public string Revenue { get; set; }
    }
}