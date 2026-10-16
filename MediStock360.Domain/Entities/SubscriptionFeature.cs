using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class SubscriptionFeature
{
    public int SubscriptionFeatureId { get; set; }

    public string FeatureCode { get; set; } = null!;

    public string FeatureName { get; set; } = null!;

    public string? DisplayTemplate { get; set; }

    public bool IsActive { get; set; }

    public bool IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; set; } = new List<SubscriptionPlanFeature>();
}
