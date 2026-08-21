
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class ClientRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; }

        [MaxLength(200)]
        [EmailAddress]
        public string CompanyEmail { get; set; }

        [Required]
        public int ComapnyTypeId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        public DateTime? SubscriptionStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? SubscriptionEndDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string? GSTNumber { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }

        public bool? IsActive { get; set; }
    }
}
