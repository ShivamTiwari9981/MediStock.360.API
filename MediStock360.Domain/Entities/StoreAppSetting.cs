using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class StoreAppSetting
{
    public long StoreAppSettingId { get; set; }

    public long ClientId { get; set; }

    public long StoreId { get; set; }

    public long AppSettingId { get; set; }

    public string? SettingValue { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual AppSetting AppSetting { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
