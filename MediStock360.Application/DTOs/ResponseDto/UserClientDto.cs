using System;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class UserClientDto
    {
        public long ClientId { get; set; }
        public Guid ClientKey { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public bool IsOnboardingCompleted { get; set; }
        public int OnboardingStep { get; set; }
    }
}
