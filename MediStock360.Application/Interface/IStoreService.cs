using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interface
{
    public interface IStoreService
    {
        ApiResponse<StoreResponseDto> AddStore(StoreRequestDto dto);
    }
}
