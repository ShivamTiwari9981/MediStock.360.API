using System.ComponentModel.DataAnnotations;


namespace MediStock360.Application.DTOs.RequestDto 
{
    public class SignupRequestDto
    {

        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; } 
        [Required]
        public string Password { get; set; }
    }
}
