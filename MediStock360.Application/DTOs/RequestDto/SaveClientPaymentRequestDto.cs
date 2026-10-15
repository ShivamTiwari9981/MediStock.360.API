using System;
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class SaveClientPaymentRequestDto
    {
        public long? ClientId { get; set; }

        public long? ClientSubscriptionId { get; set; }

        public int? SubscriptionPlanId { get; set; }

        public long? PaymentConfigurationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public string? CurrencyCode { get; set; } = "INR";

        public string? PaymentMode { get; set; } = "UPI";

        [Required(ErrorMessage = "Transaction UTR / Reference number is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Reference number must be between 3 and 150 characters.")]
        public string TransactionRefNo { get; set; } = string.Empty;

        [StringLength(250)]
        public string? PaymentNotes { get; set; }

        public DateTime? PaidAt { get; set; } = DateTime.UtcNow;
    }
}

