using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ClientPaymentResponseDto
    {
        public long ClientPaymentId { get; set; }

        public long ClientId { get; set; }

        public long ClientSubscriptionId { get; set; }

        public long PaymentConfigurationId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        [Column(TypeName = "char(3)")]
        public string CurrencyCode { get; set; } = "INR";

        [Required]
        [StringLength(150)]
        public string TransactionReference { get; set; } = string.Empty;

        [StringLength(250)]
        public string? SenderNotes { get; set; }

        // 1 = Pending, 2 = Approved, 3 = Rejected
        public byte PaymentStatus { get; set; } = 1;

        [StringLength(500)]
        public string? AdminRemarks { get; set; }

        public long? ReviewedBy { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
