using System;
using System.Collections.Generic;

namespace MediStock360.Domain.Entities;

public partial class IsSyncDatum
{
    public int ClientId { get; set; }

    public int BranchId { get; set; }

    public int SyncId { get; set; }

    public string TableName { get; set; } = null!;

    public string JsonData { get; set; } = null!;

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}
