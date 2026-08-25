using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class RolePermission
{
    public long RolePermissionId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
