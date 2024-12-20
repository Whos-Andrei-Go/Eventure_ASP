using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models;

public partial class Event
{
    public int Id { get; set; }

    public int CreatorId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual User Creator { get; set; } = null!;
}
