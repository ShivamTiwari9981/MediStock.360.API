
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class ClientRequestDto
    {
        public long ClientId { get; set; }
        public Guid ClientKey { get; set; }
        public string ClientCode { get; set; } = null!;
        [Required]
        public string? ClientName { get; set; }
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; }
        [Required]
        public string? OwnerName { get; set; }
        [Required]
        public int? BusinessTypeId { get; set; }
        [Required]

        public string? Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
        public string? Gstnumber { get; set; }
        [Required]

        public string? DrugLicenseNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }
        [Required]
        public int? CityId { get; set; }
        [Required]
        public int? StateId { get; set; }
        [Required]
        public int? CountryId { get; set; }
        [Required]
        public string? PostalCode { get; set; }

        public bool IsOnboardingCompleted { get; set; }
        [Required]
        public int OnboardingStep { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUpdate { get; set; }
    }
}
