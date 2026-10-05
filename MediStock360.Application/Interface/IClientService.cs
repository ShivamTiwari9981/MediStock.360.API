using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IClientService
    {
        Task<bool> IsClientExist();
        Task<ApiResponse<ClientResponseDto>> UpdateClient(ClientRequestDto dto);
    }
}
