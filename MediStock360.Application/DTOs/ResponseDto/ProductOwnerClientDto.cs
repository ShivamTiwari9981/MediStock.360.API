using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ProductOwnerClientDto
    {
        public long ClientId { get; set; }
        public Guid ClientKey { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string? ClientName { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
        public int? BusinessTypeId { get; set; }
        public string? BusinessTypeName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gstnumber { get; set; }
        public string? DrugLicenseNumber { get; set; }
        public string? Address { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? PostalCode { get; set; }
        public bool IsOnboardingCompleted { get; set; }
        public int OnboardingStep { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StoreCount { get; set; }
        public int UserCount { get; set; }
        public string? CurrentPlanName { get; set; }
        public byte? SubscriptionStatus { get; set; }
        public string? SubscriptionStatusText { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public byte? PaymentStatus { get; set; }
        public string? PaymentStatusText { get; set; }
    }
}

