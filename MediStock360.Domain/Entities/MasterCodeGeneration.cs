using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class MasterCodeGeneration
{
    public long MasterCodeGenerationId { get; set; }

    public long? ClientId { get; set; }

    public string CodeType { get; set; } = null!;

    public string CodePrefix { get; set; } = null!;

    public long CurrentNumber { get; set; }

    public int NumberLength { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? StoreId { get; set; }

    public virtual Client? Client { get; set; }
}
