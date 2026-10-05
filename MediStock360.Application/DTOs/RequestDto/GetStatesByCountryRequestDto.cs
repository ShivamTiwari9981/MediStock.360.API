using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class GetStatesByCountryRequestDto
    {
        [Required(ErrorMessage = "CountryId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CountryId must be a valid positive integer.")]
        public int CountryId { get; set; }

        public bool? IsActive { get; set; }
    }
}

