using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models;

public partial class Offer
{
    public int Id { get; set; }

    public int? EventId { get; set; }

    public string? Description { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
