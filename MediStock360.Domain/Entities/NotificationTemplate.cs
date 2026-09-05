using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class NotificationTemplate
{
    public long NotificationTemplateId { get; set; }

    public string TemplateCode { get; set; } = null!;

    public string TemplateName { get; set; } = null!;

    public string NotificationType { get; set; } = null!;

    public string? Subject { get; set; }

    public string Body { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
