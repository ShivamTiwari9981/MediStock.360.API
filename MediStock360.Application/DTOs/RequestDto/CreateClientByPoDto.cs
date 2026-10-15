using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class CreateClientByPoDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client/Pharmacy name is required")]
        [MaxLength(150)]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Owner name is required")]
        [MaxLength(100)]
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public string? Password { get; set; }

        public int? BusinessTypeId { get; set; }

        public string? Gstnumber { get; set; }

        public string? DrugLicenseNumber { get; set; }

        public string? Address { get; set; }

        public int? CityId { get; set; }

        public int? StateId { get; set; }

        public int? CountryId { get; set; }

        public string? PostalCode { get; set; }

        public int? SubscriptionPlanId { get; set; }

        public int MaxStores { get; set; } = 1;

        public int MaxUsers { get; set; } = 5;

        public string? InitialStoreName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

