using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class GetCitiesByStateRequestDto
    {
        [Required(ErrorMessage = "StateId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StateId must be a valid positive integer.")]
        public int StateId { get; set; }

        public bool? IsActive { get; set; }
    }
}

