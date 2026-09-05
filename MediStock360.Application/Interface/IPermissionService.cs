using System.Collections.Generic;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IPermissionService
    {
        // ------------------ Permission Master ------------------
        Task<ApiResponse<List<PermissionResponseDto>>> GetAllPermissionsAsync(string? moduleName = null, bool? isActive = null);
        Task<ApiResponse<List<ModuleGroupedPermissionsDto>>> GetPermissionsGroupedByModuleAsync();
        Task<ApiResponse<PermissionResponseDto>> GetPermissionByIdAsync(int id);
        Task<ApiResponse<PermissionResponseDto>> CreatePermissionAsync(CreatePermissionRequestDto dto);
        Task<ApiResponse<bool>> UpdatePermissionAsync(int id, UpdatePermissionRequestDto dto);
        Task<ApiResponse<bool>> TogglePermissionStatusAsync(int id);
        Task<ApiResponse<bool>> DeletePermissionAsync(int id);

        // ------------------ Role-Permission Mapping ------------------
        Task<ApiResponse<RolePermissionMappingDto>> GetPermissionsByRoleIdAsync(int roleId);
        Task<ApiResponse<bool>> AssignPermissionsToRoleAsync(AssignRolePermissionsRequestDto dto);

        // ------------------ User-Role Mapping (Multi-Role Support) ------------------
        Task<ApiResponse<UserRoleMappingDto>> GetRolesByUserIdAsync(long userId);
        Task<ApiResponse<bool>> AssignRolesToUserAsync(AssignUserRolesRequestDto dto);
        Task<ApiResponse<UserPermissionsResponseDto>> GetUserPermissionsAsync(long userId);
    }
}

