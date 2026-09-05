using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class ClientAppSetting
{
    public long ClientAppSettingId { get; set; }

    public long ClientId { get; set; }

    public long AppSettingId { get; set; }

    public string? SettingValue { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual AppSetting AppSetting { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;
}
