using System;
using System.Collections.Generic;

namespace Eventure_ASP.Models;

public partial class PaymentMethod
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string PaymentType { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;

    public DateOnly? ExpirationDate { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
