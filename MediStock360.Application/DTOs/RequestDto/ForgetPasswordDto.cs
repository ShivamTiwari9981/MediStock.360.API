using MediStock360.Application.Enums;
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto 
{
    public class ForgetPasswordDto
    {
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public OtpPurpose OtpPurpose { get; set; }
    }
}
