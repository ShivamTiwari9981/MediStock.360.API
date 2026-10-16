namespace MediStock360.Application.DTOs.RequestDto
{
    public class ApprovePaymentRequestDto
    {
        public long ClientPaymentId { get; set; }

        public bool IsApproved { get; set; } = true;

        public string? AdminRemarks { get; set; }
    }
}

