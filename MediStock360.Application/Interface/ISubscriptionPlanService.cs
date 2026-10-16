using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediStock360.Application.Interface
{
    public interface ISubscriptionPlanService
    {
        ApiResponse<List<SubscriptionPlanDto>> GetSubscriptionPlan();
        Task<ApiResponse<ConfirmSubscriptionPlanResponseDto>> ConfirmSubscriptionPlanAsync(ConfirmSubscriptionPlanRequestDto dto);
    }
}
