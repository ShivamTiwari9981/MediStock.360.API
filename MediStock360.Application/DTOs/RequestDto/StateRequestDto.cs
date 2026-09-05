using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class StateRequestDto
    {
        public Guid StateId { get; set; }
        [Required]
        public Guid CountryId { get; set; }
        [Required]
        public string StateName { get; set; }
    }
}
