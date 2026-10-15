using System;
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class ConfirmSubscriptionPlanRequestDto
    {
        [Required]
        public int SubscriptionPlanId { get; set; }

        public long? ClientId { get; set; }

        public byte? BillingCycle { get; set; }

        public decimal? Price { get; set; }

        public string? PlanCode { get; set; }

        public string? PlanName { get; set; }

        public string? CurrencyCode { get; set; }

        public bool IsConfirmed { get; set; } = true;

        public DateTime? ConfirmedAt { get; set; } = DateTime.UtcNow;
    }
}

