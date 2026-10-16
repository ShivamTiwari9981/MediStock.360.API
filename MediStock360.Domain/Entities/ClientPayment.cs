using System;
using System.Collections.Generic;

namespace MediStock360.Infrastructure;

public partial class ClientPayment
{
    public long ClientPaymentId { get; set; }

    public long ClientId { get; set; }

    public long ClientSubscriptionId { get; set; }

    public long PaymentConfigurationId { get; set; }

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string TransactionReference { get; set; } = null!;

    public string? SenderNotes { get; set; }

    public byte PaymentStatus { get; set; }

    public string? AdminRemarks { get; set; }

    public long? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime SubmittedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}
