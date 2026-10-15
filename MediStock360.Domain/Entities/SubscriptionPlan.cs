using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class SubscriptionPlan
{
    public int SubscriptionPlanId { get; set; }

    public string PlanCode { get; set; } = null!;

    public string PlanName { get; set; } = null!;

    public string? Description { get; set; }

    public byte BillingCycle { get; set; }

    public decimal Price { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public int? TrialDays { get; set; }

    public bool IsActive { get; set; }

    public bool IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; set; } = new List<SubscriptionPlanFeature>();
}
