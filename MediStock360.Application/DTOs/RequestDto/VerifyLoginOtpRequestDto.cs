using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class VerifyLoginOtpRequestDto
    {
        [Required(ErrorMessage = "Email or Username is required.")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "OTP must be between 4 and 10 digits.")]
        public string Otp { get; set; } = string.Empty;
    }
}

