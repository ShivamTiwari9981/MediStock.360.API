using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class CityRequestDto
    {
        public int CityId { get; set; }

        [Required]
        public int StateId { get; set; }

        public int CountryId { get; set; }

        [Required]
        public string CityName { get; set; }

        public bool? IsActive { get; set; }
    }
}

