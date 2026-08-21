using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class UserRequestDto
    {
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string UserEmail { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? ProfileImagePath { get; set; }
    }
}
