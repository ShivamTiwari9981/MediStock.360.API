using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Enums;

namespace MediStock360.Application.Interfaces
{
    public interface IOTPService
    {
        Task SaveOTP(long userId, string userEmail, string otp, OtpPurpose purpose);
        Task<ApiResponse<bool>> SendOtpAsync(string userEmail, OtpPurpose purpose);
        Task<ApiResponse<bool>> VerfyOtpAsync(string userEmail, string otp);
        Task<ApiResponse<bool>> ResendOtp(string userEmail, OtpPurpose purpose);
    }
}
