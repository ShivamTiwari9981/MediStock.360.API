using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediStock360.Application.Common;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediStock360.Application.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly TelegramSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TelegramBotService> _logger;

        public TelegramBotService(IOptions<TelegramSettings> options, HttpClient httpClient, ILogger<TelegramBotService> logger)
        {
            _settings = options.Value ?? new TelegramSettings();
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> SendMessageAsync(string message)
        {
            if (!_settings.IsEnabled || string.IsNullOrWhiteSpace(_settings.BotToken) || string.IsNullOrWhiteSpace(_settings.ChatId))
            {
                _logger.LogWarning("Telegram Bot: Notification skipped because TelegramSettings is disabled or BotToken/ChatId is empty.");
                return false;
            }

            try
            {
                string url = $"https://api.telegram.org/bot{_settings.BotToken}/sendMessage";
                var payload = new
                {
                    chat_id = _settings.ChatId,
                    text = message,
                    parse_mode = "HTML"
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Telegram Bot API returned error ({StatusCode}): {Body}", response.StatusCode, errorBody);
                    return false;
                }

                _logger.LogInformation("Telegram notification sent successfully to chat {ChatId}.", _settings.ChatId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Telegram Bot error while sending message.");
                return false;
            }
        }

        public async Task<bool> SendPaymentNotificationAsync(ClientPaymentNotificationDto notification)
        {
            var sb = new StringBuilder();
            sb.AppendLine("🔔 <b>NEW CLIENT PAYMENT SUBMITTED</b> 🔔");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine($"🏢 <b>Client:</b> {EscapeHtml(notification.ClientName)} (<i>{EscapeHtml(notification.CompanyName)}</i>)");
            sb.AppendLine($"📋 <b>Plan:</b> {EscapeHtml(notification.PlanName)}");
            sb.AppendLine($"💰 <b>Amount:</b> ₹{notification.Amount:N2} {EscapeHtml(notification.CurrencyCode)}");
            sb.AppendLine($"💳 <b>Payment Mode:</b> {EscapeHtml(notification.PaymentMode)}");
            sb.AppendLine($"🔢 <b>UTR / Ref No:</b> <code>{EscapeHtml(notification.TransactionReference)}</code>");
            if (!string.IsNullOrWhiteSpace(notification.SenderNotes))
            {
                sb.AppendLine($"📝 <b>Notes:</b> {EscapeHtml(notification.SenderNotes)}");
            }
            sb.AppendLine($"📅 <b>Time:</b> {notification.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine("⏳ <b>Status:</b> <b>Pending Admin Approval</b> (Client NOT activated)");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine("👉 <i>Please review UTR/Receipt in database/portal and approve client.</i>");

            return await SendMessageAsync(sb.ToString());
        }

        public async Task<bool> SendPaymentApprovalNotificationAsync(string clientName, string companyName, string planName, decimal amount, bool isApproved, string? remarks)
        {
            var sb = new StringBuilder();
            if (isApproved)
            {
                sb.AppendLine("✅ <b>SUBSCRIPTION PAYMENT APPROVED</b> ✅");
                sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine($"🏢 <b>Client:</b> {EscapeHtml(clientName)} (<i>{EscapeHtml(companyName)}</i>)");
                sb.AppendLine($"📋 <b>Plan:</b> {EscapeHtml(planName)}");
                sb.AppendLine($"💰 <b>Amount:</b> ₹{amount:N2}");
                sb.AppendLine("🚀 <b>Status:</b> <b>ACTIVE</b> (Client account activated)");
            }
            else
            {
                sb.AppendLine("❌ <b>SUBSCRIPTION PAYMENT REJECTED</b> ❌");
                sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine($"🏢 <b>Client:</b> {EscapeHtml(clientName)} (<i>{EscapeHtml(companyName)}</i>)");
                sb.AppendLine($"📋 <b>Plan:</b> {EscapeHtml(planName)}");
                sb.AppendLine($"💰 <b>Amount:</b> ₹{amount:N2}");
                sb.AppendLine("🚫 <b>Status:</b> <b>REJECTED</b> (Client not activated)");
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                sb.AppendLine($"📝 <b>Remarks:</b> {EscapeHtml(remarks)}");
            }
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            return await SendMessageAsync(sb.ToString());
        }

        private static string EscapeHtml(string? input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }
}

