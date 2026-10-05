using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class StateRequestDto
    {
        public int StateId { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        public string StateName { get; set; }

        public bool? IsActive { get; set; }
    }
}

