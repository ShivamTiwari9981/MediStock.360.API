using MediStock360.Application.Common;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;


namespace MediStock360.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task<ApiResponse<bool>> SendEmailOTP(string userEmail, string otp)
        {
            try
            {
                string serverName = _settings.SmtpServer;
                int port = _settings.Port;
                string username = _settings.Username;
                string password = _settings.Password;
                string fromEmail = _settings.FromEmail;
                string appName = _settings.DisplayName;

                var fromAddress = new MailAddress(fromEmail, appName);
                var toAddress = new MailAddress(userEmail);

                const string subject = "Your Verification Code";

                string body = $@"
                    <h3>{appName}</h3>
                    <p>Your One-Time Password (OTP) is:</p>
                    <h2>{otp}</h2>
                    <p>This OTP is valid for 10 minutes.</p>
                ";

                using var smtp = new SmtpClient
                {
                    Host = serverName,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(message);

                return ApiResponse<bool>.Success(true, "OTP sent successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }
    }
}