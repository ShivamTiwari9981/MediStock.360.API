using System.Collections.Generic;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IProductOwnerService
    {
        Task<ApiResponse<ProductOwnerDashboardDto>> GetDashboardStatsAsync();
        Task<ApiResponse<List<ProductOwnerClientDto>>> GetAllClientsAsync(string? searchTerm = null, bool? isActive = null);
        Task<ApiResponse<ProductOwnerClientDto>> GetClientDetailsAsync(long clientId);
        Task<ApiResponse<ProductOwnerClientDto>> CreateClientAsync(CreateClientByPoDto dto);
        Task<ApiResponse<bool>> UpdateClientStatusAsync(long clientId, bool isActive);
        Task<ApiResponse<ClientConfigurationDto>> GetClientConfigurationAsync(long clientId);
        Task<ApiResponse<ClientConfigurationDto>> SaveClientConfigurationAsync(ClientConfigurationDto dto);
        Task<ApiResponse<List<ProductOwnerStoreDto>>> GetAllStoresAsync(long? clientId = null, string? searchTerm = null);
        Task<ApiResponse<List<PendingApprovalDto>>> GetApprovalsAsync(byte? status = null);
        Task<ApiResponse<bool>> ProcessApprovalActionAsync(ApprovePaymentActionDto dto);
    }
}

