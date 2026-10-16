using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string MethodCode { get; set; } = null!;

    public string MethodName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<PaymentConfiguration> PaymentConfigurations { get; set; } = new List<PaymentConfiguration>();
}
