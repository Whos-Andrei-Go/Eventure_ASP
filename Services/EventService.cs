using System.Collections.Generic;
using Eventure_ASP.Data;
using Eventure_ASP.Models;

namespace Eventure_ASP.Services
{
    public class EventService
    {
        private readonly EtsDbContext _context; // Your DbContext

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
            .Where(ticket => ticket.TicketType.EventId == eventId) // Filter tickets by EventId
            .Sum(ticket => ticket.TicketType.Price); // Sum the total price based on quantity

            return revenue;
        }
    }
}