using MediStock360.Application.Common;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using NotificationTemplateConstants = MediStock360.Application.Common.constaints.NotificationTemplates;
namespace MediStock360.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IOptions<EmailSettings> options, IUnitOfWork unitOfWork)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse<bool>> SendEmailOTP(string userEmail, string otp, string userName = "User", int expiryMinutes = 10)
        {
            try
            {
                var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "UserName", string.IsNullOrWhiteSpace(userName) ? "User" : userName },
                    { "OTP", otp },
                    { "ExpiryMinutes", expiryMinutes.ToString() },
                    { "AppName", _settings.DisplayName ?? "MediStock360" }
                };

                // Check if notification template exists in database
                var template = await _unitOfWork.NotificationTemplateRepository.FirstOrDefaultAsync(
                    t => t.TemplateCode == NotificationTemplateConstants.EmailType && t.IsActive
                );

                if (template != null)
                {
                    return await SendNotificationEmailAsync(userEmail, NotificationTemplateConstants.EmailType, placeholders);
                }

                // Fallback template if not found in database
                string appName = _settings.DisplayName ?? "MediStock360";
                string subject = $"{appName} - Your Verification Code";
                string body = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;"">
                        <h2 style=""color: #2b6cb0;"">{appName}</h2>
                        <p>Hello <strong>{placeholders["UserName"]}</strong>,</p>
                        <p>Your One-Time Password (OTP) for MediStock360 is:</p>
                        <div style=""background-color: #f7fafc; padding: 15px; text-align: center; border-radius: 6px; margin: 20px 0;"">
                            <span style=""font-size: 28px; font-weight: bold; letter-spacing: 4px; color: #2d3748;"">{otp}</span>
                        </div>
                        <p>This OTP is valid for <strong>{expiryMinutes} minutes</strong>.</p>
                        <p>Please do not share this OTP with anyone.</p>
                        <p style=""color: #718096; font-size: 12px; margin-top: 30px;"">If you did not request this verification code, please ignore this email.</p>
                        <p>Regards,<br/><strong>{appName} Team</strong></p>
                    </div>";

                return await SendEmailAsync(userEmail, subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, $"Failed to send OTP email: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SendNotificationEmailAsync(string toEmail,string templateCode, Dictionary<string, string> placeholders)
        {
            try
            {
                var template = await _unitOfWork.NotificationTemplateRepository.FirstOrDefaultAsync(
                    t => t.TemplateCode == templateCode && t.IsActive && t.NotificationType == "EMAIL"
                );

                if (template == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Notification template '{templateCode}' was not found or is inactive.");
                }

                string subject = ReplacePlaceholders(template.Subject ?? "Notification", placeholders);
                string body = ReplacePlaceholders(template.Body, placeholders);
                string formattedBody = FormatHtmlBody(body);

                return await SendEmailAsync(toEmail, subject, formattedBody, isHtml: true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, $"Error sending notification email: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SendEmailAsync(string toEmail,string subject, string body, bool isHtml = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    return ApiResponse<bool>.Fail(1, "Recipient email is required.");
                }

                string serverName = _settings.SmtpServer;
                int port = _settings.Port;
                string username = _settings.UserName;
                string password = _settings.Password;
                string fromEmail = _settings.FromEmail;
                string appName = _settings.DisplayName ?? "MediStock360";

                var fromAddress = new MailAddress(fromEmail, appName);
                var toAddress = new MailAddress(toEmail);

                using var smtp = new SmtpClient
                {
                    Host = serverName,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, password)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                await smtp.SendMailAsync(message);

                return ApiResponse<bool>.Success(true, "Email sent successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, $"Failed to send email: {ex.Message}");
            }
        }

        private string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            string result = template;

            if (placeholders != null)
            {
                foreach (var kvp in placeholders)
                {
                    result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                    result = result.Replace($"{{{kvp.Key}}}", kvp.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                }
            }

            result = result.Replace("{{AppName}}", _settings.DisplayName ?? "MediStock360", StringComparison.OrdinalIgnoreCase);
            result = result.Replace("{{Year}}", DateTime.UtcNow.Year.ToString(), StringComparison.OrdinalIgnoreCase);

            return result;
        }

        private static string FormatHtmlBody(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return string.Empty;

            if (!body.Contains("<html", StringComparison.OrdinalIgnoreCase) &&
                !body.Contains("<div", StringComparison.OrdinalIgnoreCase) &&
                !body.Contains("<p", StringComparison.OrdinalIgnoreCase))
            {
                string formattedContent = body.Replace("\r\n", "<br/>").Replace("\n", "<br/>");
                return $@"
                    <div style=""font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6; color: #333333; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;"">
                        {formattedContent}
                    </div>";
            }

            return body;
        }
    }
}