using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class OtpVerificationRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string email { get; set; }

        public string otp { get; set; }
    }
}
