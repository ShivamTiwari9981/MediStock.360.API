using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class CityRequestDto
    {
        public Guid CityId { get; set; } = Guid.Empty;

        [Required]
        public Guid StateId { get; set; }

        [Required]
        public string CityName { get; set; }
    }
}
