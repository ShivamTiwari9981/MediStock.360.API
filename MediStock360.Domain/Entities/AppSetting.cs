using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class AppSetting
{
    public long AppSettingId { get; set; }

    public long ClientId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string SettingValue { get; set; } = null!;

    public string? DataType { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}
