namespace Eventure_ASP.Models
{
    public class DashboardViewModel
    {
        public List<Event> UpcomingEvents { get; set; } = new List<Event>();

        public List<Event> YourEvents { get; set; } = new List<Event>();
    }
}
