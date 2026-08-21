using System.ComponentModel.DataAnnotations;


namespace MediStock360.Application.DTOs.RequestDto
{
    public class SignupRequestDto
    {
        [Required]
        public string UserName { get; set; } 
        [Required]
        public string UserEmail { get; set; } 
        [Required]
        public string Password { get; set; }
    }
}
