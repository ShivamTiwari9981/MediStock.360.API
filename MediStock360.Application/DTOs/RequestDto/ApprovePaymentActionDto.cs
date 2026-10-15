using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class ApprovePaymentActionDto
    {
        [Required]
        public long ClientPaymentId { get; set; }

        public bool IsApproved { get; set; } = true;

        public string? AdminRemarks { get; set; }
    }
}

