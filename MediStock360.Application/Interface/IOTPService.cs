using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IOTPService
    {
        Task SaveOTP(long userId, string userEmail, string otp);
        Task<ApiResponse<bool>> SendOtpAsync(string userEmail);
        Task<ApiResponse<bool>> VerfyOtpAsync(string userEmail, string otp);
        Task<ApiResponse<bool>> ResendOtp(string userEmail);
    }
}
