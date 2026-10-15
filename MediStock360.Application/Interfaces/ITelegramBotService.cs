using System.Threading.Tasks;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface ITelegramBotService
    {
        Task<bool> SendMessageAsync(string message);
        Task<bool> SendPaymentNotificationAsync(ClientPaymentNotificationDto notification);
        Task<bool> SendPaymentApprovalNotificationAsync(string clientName, string companyName, string planName, decimal amount, bool isApproved, string? remarks);
    }
}

