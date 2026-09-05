using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class BusinessType
{
    public int BusinessTypeId { get; set; }

    public string BusinessTypeCode { get; set; } = null!;

    public string BusinessTypeName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
