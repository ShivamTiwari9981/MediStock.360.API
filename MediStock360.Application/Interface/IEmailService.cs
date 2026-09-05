using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IEmailService
    {
        Task<ApiResponse<bool>> SendEmailOTP(string userEmail, string otp, string userName = "User", int expiryMinutes = 10);
        Task<ApiResponse<bool>> SendNotificationEmailAsync(string toEmail, string templateCode, Dictionary<string, string> placeholders);
        Task<ApiResponse<bool>> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
