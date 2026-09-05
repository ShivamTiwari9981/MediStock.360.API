using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class DatabaseVersion
{
    public long DatabaseVersionId { get; set; }

    public int VersionNumber { get; set; }

    public string? Description { get; set; }

    public DateTime AppliedAt { get; set; }
}
