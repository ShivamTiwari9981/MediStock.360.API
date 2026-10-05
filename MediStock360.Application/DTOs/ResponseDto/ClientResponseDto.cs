namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ClientResponseDto
    {
        public long ClientId { get; set; }

        public Guid ClientKey { get; set; }

        public string ClientCode { get; set; } = null!;

        public string? ClientName { get; set; }

        public string CompanyName { get; set; } = null!;

        public string? OwnerName { get; set; }

        public int? BusinessTypeId { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Gstnumber { get; set; }

        public string? DrugLicenseNumber { get; set; }

        public string? Address { get; set; }

        public int? CityId { get; set; }

        public int? StateId { get; set; }

        public int? CountryId { get; set; }

        public string? PostalCode { get; set; }

        public bool IsOnboardingCompleted { get; set; }

        public int OnboardingStep { get; set; }

        public bool? IsActive { get; set; }
    }
}
