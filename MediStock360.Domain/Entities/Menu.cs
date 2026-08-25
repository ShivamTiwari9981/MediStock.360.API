using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class Menu
{
    public int MenuId { get; set; }

    public int? ParentMenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public string MenuIcon { get; set; } = null!;

    public string RouterLink { get; set; } = null!;

    public string? PermissionCode { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsVisible { get; set; }

    public int? IsActive { get; set; }

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<Menu> InverseParentMenu { get; set; } = new List<Menu>();

    public virtual Menu? ParentMenu { get; set; }
}
