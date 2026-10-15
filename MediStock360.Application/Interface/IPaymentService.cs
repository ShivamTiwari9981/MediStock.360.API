using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using System.Threading.Tasks;

namespace MediStock360.Application.Interface
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDto>> GetPaymentMethod();
        Task<ApiResponse<ClientPaymentResponseDto>> SubmitClientPaymentAsync(SaveClientPaymentRequestDto dto);
        Task<ApiResponse<ClientPaymentResponseDto>> ApproveClientPaymentAsync(ApprovePaymentRequestDto dto);
        Task<ApiResponse<bool>> TestTelegramBotAsync(string testMessage);
    }
}
