using System.Collections.Generic;
using System.Linq;
using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventure_ASP.Services
{
    public class TicketService
    {
        private readonly EtsDbContext _context;

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
            return _context.Tickets.Count(ticket => ticket.TicketType.EventId == eventId);
        }

        public int GetTotalTicketsSold()
        {
            return _context.Tickets.Count();
        }

        public decimal GetTotalRevenue()
        {
            return _context.Tickets
                .Include(t => t.TicketType) // Ensure TicketType data is loaded
                .Sum(t => t.TicketType.Price); // Sum the prices of all ticket types
        }
    }
}