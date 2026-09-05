

using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
 
    public class OtpResendRequest
    {
        [Required]
        [MaxLength(200)]
        public string email { get; set; }
    }
}
