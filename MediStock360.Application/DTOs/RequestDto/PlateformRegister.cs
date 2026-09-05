using System.ComponentModel.DataAnnotations;


namespace MediStock360.Application.DTOs.RequestDto
{
    public class PlateformRegister
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
