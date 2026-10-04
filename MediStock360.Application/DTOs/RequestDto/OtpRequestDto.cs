using MediStock360.Application.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class OtpRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string UserEmail { get; set; }
        [Required]
        public OtpPurpose Purpose { get; set; }
    }
}
