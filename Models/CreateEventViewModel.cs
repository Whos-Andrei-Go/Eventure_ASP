using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models
{
    public class CreateEventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = String.Empty;
        public string EventLocation { get; set; } = String.Empty;
        public string EventDescription { get; set; } = String.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<TicketType> TicketTypes { get; set; } = new List<TicketType>();
    }
}