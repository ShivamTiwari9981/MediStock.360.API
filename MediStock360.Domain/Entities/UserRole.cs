using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class UserRole
{
    public long UserRoleId { get; set; }

    public long ClientId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
