using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TicketTypeId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public string? Status { get; set; }

    public string? QrCode { get; set; }

    public virtual TicketType TicketType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
