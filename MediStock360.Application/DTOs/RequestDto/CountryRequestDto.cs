using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class CountryRequestDto
    {
        public Guid CountryId { get; set; }

        [Required]
        public string CountryName { get; set; }
    }
}
