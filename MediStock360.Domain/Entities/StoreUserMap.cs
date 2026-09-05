using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class StoreUserMap
{
    public long StoreUserMapId { get; set; }

    public long ClientId { get; set; }

    public long StoreId { get; set; }

    public long UserId { get; set; }

    public bool IsDefaultStore { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
