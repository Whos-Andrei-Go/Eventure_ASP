using System;

namespace Eventure_ASP.Models
{
    public class ManageEventsViewModel
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalTicketsSold { get; set; }
    }
}