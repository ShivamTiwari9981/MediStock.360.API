using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class ResetPasswordDto
    {
        public string UserEmail { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
