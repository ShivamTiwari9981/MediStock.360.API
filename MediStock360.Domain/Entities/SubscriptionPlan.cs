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

    public int MaxStores { get; set; }

    public int MaxUsers { get; set; }

    public int? MaxProducts { get; set; }

    public int? MaxInvoicesPerMonth { get; set; }

    public int? MaxCustomers { get; set; }

    public int? MaxSuppliers { get; set; }

    public bool IsInventoryEnabled { get; set; }

    public bool IsPurchaseEnabled { get; set; }

    public bool IsSalesEnabled { get; set; }

    public bool IsReportsEnabled { get; set; }

    public bool IsAienabled { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSynced { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();
}
