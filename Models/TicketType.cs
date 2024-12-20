using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models;

public partial class TicketType
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
