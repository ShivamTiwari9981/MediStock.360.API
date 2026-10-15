using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class PendingApprovalDto
    {
        public long ClientPaymentId { get; set; }
        public long ClientId { get; set; }
        public Guid ClientKey { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }
        public long ClientSubscriptionId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "INR";
        public string PaymentMethodName { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string? SenderNotes { get; set; }
        public byte PaymentStatus { get; set; }
        public string PaymentStatusText { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string? AdminRemarks { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}

