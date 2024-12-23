using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models
{
    public class EventViewModel
    {
        public Event Event { get; set; }
        public List<TicketType> TicketTypes { get; set; }
        public int TicketsSold { get; set; }
        public decimal Revenue { get; set; }

        // Separate properties for date and time
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
    }
}