using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UpcomingEventsCount { get; set; }
        public string TopSellingEventName { get; set; }
        public int TotalRegisteredUsers { get; set; }
    }
}