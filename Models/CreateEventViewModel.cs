using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models
{
    public class CreateEventViewModel
    {
        public string EventName { get; set; } = String.Empty;
        public string EventLocation { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<TicketType> TicketTypes { get; set; } = new List<TicketType>();
    }
}