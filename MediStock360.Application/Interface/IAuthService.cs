using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IAuthService
    {
        ApiResponse<string> SignUp(SignupRequestDto dto);
        Task<ApiResponse<string>> SignUpAsync(SignupRequestDto dto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<ApiResponse<LoginResponseDto>> SwitchStoreAsync(SwitchStoreRequestDto dto);
        Task<ApiResponse<LoginResponseDto>> SwitchClientAsync(SwitchClientRequestDto dto);
        Task<ApiResponse<bool>> ResetPassword(string userEmail, string password);
    }
}

