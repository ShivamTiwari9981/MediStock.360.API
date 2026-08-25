
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class StateResponseDto
    {
        public Guid StateId { get; set; } 

        public Guid CountryId { get; set; }
        public string StateName { get; set; }
        public bool? IsActive { get; set; }
    }
}
