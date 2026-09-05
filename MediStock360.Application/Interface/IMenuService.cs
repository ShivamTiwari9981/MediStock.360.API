using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediStock360.Application.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuResponseDto>> GetAccessibleMenusForUserAsync(long userId, List<string>? roleNames = null, List<string>? permissionCodes = null, long? clientId = null, long? storeId = null);
        Task<ApiResponse<List<MenuResponseDto>>> GetNavMenusForCurrentUserAsync();
        Task<ApiResponse<List<MenuResponseDto>>> GetAccessibleMenusForRoleAsync(int roleId);
        Task<ApiResponse<List<MenuResponseDto>>> GetAllMenusTreeAsync();
        Task<ApiResponse<List<MenuResponseDto>>> GetAllMenusFlatAsync();
        Task<ApiResponse<MenuResponseDto>> GetMenuByIdAsync(int id);
        Task<ApiResponse<MenuResponseDto>> CreateMenuAsync(CreateMenuRequestDto dto);
        Task<ApiResponse<bool>> UpdateMenuAsync(int id, UpdateMenuRequestDto dto);
        Task<ApiResponse<bool>> ToggleMenuStatusAsync(int id);
        Task<ApiResponse<bool>> DeleteMenuAsync(int id);
        Task<ApiResponse<bool>> SeedDefaultMenusAsync();
        //List<MenuResponseDto> BuildMenuTreeByProc(List<MenuResponseDto> allMenus);
        List<MenuResponseDto> BuildMenuTree(List<Menu> allMenus, HashSet<int>? permittedMenuIds = null);


    }
}

