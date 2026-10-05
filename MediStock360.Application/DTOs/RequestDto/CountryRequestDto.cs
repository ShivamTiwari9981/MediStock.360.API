using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class CountryRequestDto
    {
        public int CountryId { get; set; }

        [Required]
        public string CountryName { get; set; }

        public bool? IsActive { get; set; }
    }
}

