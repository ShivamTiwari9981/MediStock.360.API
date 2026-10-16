using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ClientPaymentNotificationDto
    {
        public long ClientPaymentId { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "INR";
        public string PaymentMode { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string? SenderNotes { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}

