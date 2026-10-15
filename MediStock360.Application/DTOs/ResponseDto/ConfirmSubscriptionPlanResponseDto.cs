using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ConfirmSubscriptionPlanResponseDto
    {
        public long ClientSubscriptionId { get; set; }

        public long ClientId { get; set; }

        public int SubscriptionPlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public string PlanCode { get; set; } = string.Empty;

        public byte BillingCycle { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "INR";

        public byte Status { get; set; } // 1 = Trial/Pending, 2 = Active

        public byte PaymentStatus { get; set; } // 1 = Pending, 2 = Paid

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool RequiresPayment { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}

