using System;
using System.Collections.Generic;

namespace MediStock360.Domain.Entities;

public partial class ClientSubscription
{
    public long ClientSubscriptionId { get; set; }

    public long ClientId { get; set; }

    public int SubscriptionPlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public byte Status { get; set; }

    public byte BillingCycle { get; set; }

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public bool AutoRenew { get; set; }

    public byte PaymentStatus { get; set; }

    public string? TransactionReference { get; set; }

    public bool IsTrial { get; set; }

    public DateTime? TrialEndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
