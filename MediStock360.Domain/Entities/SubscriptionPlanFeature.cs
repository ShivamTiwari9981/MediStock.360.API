using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class SubscriptionPlanFeature
{
    public long SubscriptionPlanFeatureId { get; set; }

    public int SubscriptionPlanId { get; set; }

    public int SubscriptionFeatureId { get; set; }

    public int? FeatureValue { get; set; }

    public bool IsEnabled { get; set; }

    public int DisplayOrder { get; set; }

    public virtual SubscriptionFeature SubscriptionFeature { get; set; } = null!;

    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
