using System.Collections.Generic;
using Eventure_ASP.Data;
using Eventure_ASP.Models;

namespace Eventure_ASP.Services
{
    public class TicketService
    {
        private readonly EtsDbContext _context; // Your DbContext

        public TicketService(EtsDbContext context)
        {
            _context = context;
        }

        public List<TicketType> GetTicketTypesByEventId(int eventId)
        {
            return _context.TicketTypes.Where(t => t.EventId == eventId).ToList();
        }

        public TicketType GetTicketTypeById(int ticketTypeId)
        {
            return _context.TicketTypes.Find(ticketTypeId);
        }

        public bool CreateTicketType(TicketType ticketType)
        {
            _context.TicketTypes.Add(ticketType);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateTicketType(TicketType ticketType)
        {
            _context.TicketTypes.Update(ticketType);
            return _context.SaveChanges() > 0;
        }

        public int GetEventTicketsSold(int eventId)
        {
            // Count the number of tickets sold for the event by checking the TicketType's EventId
            var ticketsSold = _context.Tickets
                .Count(ticket => ticket.TicketType.EventId == eventId); // Count tickets where EventId matches

            return ticketsSold;
        }
    }
}