using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace MediStock360.Application.Services
{
    public class AuthService :  IAuthService
    {
        private readonly IConfiguration _configuration;
        
       
    }
}