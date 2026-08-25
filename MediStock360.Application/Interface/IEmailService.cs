using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IEmailService
    {
        Task<ApiResponse<bool>> SendEmailOTP(string userEmail, string otp);
    }
}
