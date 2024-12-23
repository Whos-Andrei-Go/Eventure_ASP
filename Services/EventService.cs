using System.Collections.Generic;
using System.Linq;
using Eventure_ASP.Data;
using Eventure_ASP.Models;

namespace Eventure_ASP.Services
{
    public class EventService
    {
        private readonly EtsDbContext _context;

        public EventService(EtsDbContext context)
        {
            _context = context;
        }

        public Event GetEventById(int eventId)
        {
            return _context.Events.Find(eventId);
        }

        public bool DeleteEvent(int eventId)
        {
            var eventToDelete = _context.Events.Find(eventId);
            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public decimal GetEventRevenue(int eventId)
        {
            var revenue = _context.Tickets
                .Where(ticket => ticket.TicketType.EventId == eventId)
                .Sum(ticket => ticket.TicketType.Price);
            return revenue;
        }

        public int GetUpcomingEventsCount()
        {
            return _context.Events.Count(e => e.StartTime > DateTime.Now);
        }

        public string GetTopSellingEventName()
        {
            var topEvent = _context.Tickets
                .GroupBy(t => t.TicketType.EventId)
                .Select(g => new
                {
                    EventId = g.Key,
                    TotalSold = g.Count() 
                })
                .OrderByDescending(x => x.TotalSold)
                .FirstOrDefault();

            if (topEvent != null)
            {
                var eventEntity = _context.Events.Find(topEvent.EventId);
                return eventEntity?.Name ?? "N/A";
            }

            return "N/A";
        }

        public List<Event> GetUpcomingEvents()
        {
            return _context.Events
                .Where(e => e.StartTime > DateTime.Now)
                .ToList();
        }

        public bool UpdateEvent(Event eventItem)
        {
            var existingEvent = _context.Events.Find(eventItem.Id);
            if (existingEvent != null)
            {
                existingEvent.Name = eventItem.Name;
                existingEvent.Description = eventItem.Description;
                existingEvent.Location = eventItem.Location;
                existingEvent.StartTime = eventItem.StartTime;
                existingEvent.EndTime = eventItem.EndTime;

                _context.SaveChanges();
                return true; // Return true if the update was successful
            }

            return false; // Return false if the event was not found
        }
    }
}