using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;

namespace MediStock360.Application.Services
{
    public class PermissionService : BaseService, IPermissionService
    {
        public PermissionService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {
        }

        #region Permission Master

        public async Task<ApiResponse<List<PermissionResponseDto>>> GetAllPermissionsAsync(string? moduleName = null, bool? isActive = null)
        {
            try
            {
                var permissions = await _unitOfWork.PermissionRepository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(moduleName))
                {
                    permissions = permissions
                        .Where(p => p.ModuleName.Equals(moduleName.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (isActive.HasValue)
                {
                    permissions = permissions
                        .Where(p => p.IsActive == isActive.Value)
                        .ToList();
                }

                var response = permissions
                    .OrderBy(p => p.ModuleName)
                    .ThenBy(p => p.PermissionName)
                    .Select(p => new PermissionResponseDto
                    {
                        PermissionId = p.PermissionId,
                        PermissionCode = p.PermissionCode,
                        PermissionName = p.PermissionName,
                        ModuleName = p.ModuleName,
                        Description = p.Description,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt
                    })
                    .ToList();

                return ApiResponse<List<PermissionResponseDto>>.Success(response, "Permissions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PermissionResponseDto>>.Fail(500, $"Error retrieving permissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ModuleGroupedPermissionsDto>>> GetPermissionsGroupedByModuleAsync()
        {
            try
            {
                var permissions = await _unitOfWork.PermissionRepository.WhereAsync(p => p.IsActive);

                var grouped = permissions
                    .GroupBy(p => p.ModuleName)
                    .OrderBy(g => g.Key)
                    .Select(g => new ModuleGroupedPermissionsDto
                    {
                        ModuleName = g.Key,
                        TotalPermissions = g.Count(),
                        Permissions = g.OrderBy(p => p.PermissionName).Select(p => new PermissionResponseDto
                        {
                            PermissionId = p.PermissionId,
                            PermissionCode = p.PermissionCode,
                            PermissionName = p.PermissionName,
                            ModuleName = p.ModuleName,
                            Description = p.Description,
                            IsActive = p.IsActive,
                            CreatedAt = p.CreatedAt
                        }).ToList()
                    })
                    .ToList();

                return ApiResponse<List<ModuleGroupedPermissionsDto>>.Success(grouped, "Grouped permissions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ModuleGroupedPermissionsDto>>.Fail(500, $"Error retrieving grouped permissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PermissionResponseDto>> GetPermissionByIdAsync(int id)
        {
            try
            {
                var permission = await _unitOfWork.PermissionRepository.FirstOrDefaultAsync(p => p.PermissionId == id);
                if (permission == null)
                {
                    return ApiResponse<PermissionResponseDto>.Fail(1, $"Permission with ID {id} not found.");
                }

                var response = new PermissionResponseDto
                {
                    PermissionId = permission.PermissionId,
                    PermissionCode = permission.PermissionCode,
                    PermissionName = permission.PermissionName,
                    ModuleName = permission.ModuleName,
                    Description = permission.Description,
                    IsActive = permission.IsActive,
                    CreatedAt = permission.CreatedAt
                };

                return ApiResponse<PermissionResponseDto>.Success(response, "Permission retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<PermissionResponseDto>.Fail(500, $"Error retrieving permission: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PermissionResponseDto>> CreatePermissionAsync(CreatePermissionRequestDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<PermissionResponseDto>.Fail(1, "Invalid request payload.");

                var normalizedCode = dto.PermissionCode.Trim().ToUpper();

                // Check for duplicate permission code
                var exists = await _unitOfWork.PermissionRepository.AnyAsync(
                    p => p.PermissionCode.ToUpper() == normalizedCode
                );

                if (exists)
                {
                    return ApiResponse<PermissionResponseDto>.Fail(1, $"Permission with code '{normalizedCode}' already exists.");
                }

                var entity = new Permission
                {
                    PermissionCode = normalizedCode,
                    PermissionName = dto.PermissionName.Trim(),
                    ModuleName = dto.ModuleName.Trim(),
                    Description = dto.Description?.Trim(),
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PermissionRepository.AddAsync(entity);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                {
                    return ApiResponse<PermissionResponseDto>.Fail(1, "Failed to save the new permission.");
                }

                var response = new PermissionResponseDto
                {
                    PermissionId = entity.PermissionId,
                    PermissionCode = entity.PermissionCode,
                    PermissionName = entity.PermissionName,
                    ModuleName = entity.ModuleName,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                    CreatedAt = entity.CreatedAt
                };

                return ApiResponse<PermissionResponseDto>.Success(response, "Permission created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<PermissionResponseDto>.Fail(500, $"Error creating permission: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdatePermissionAsync(int id, UpdatePermissionRequestDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<bool>.Fail(1, "Invalid request payload.");

                var permission = await _unitOfWork.PermissionRepository.FirstOrDefaultAsync(p => p.PermissionId == id);
                if (permission == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Permission with ID {id} not found.");
                }

                permission.PermissionName = dto.PermissionName.Trim();
                permission.ModuleName = dto.ModuleName.Trim();
                permission.Description = dto.Description?.Trim();
                permission.IsActive = dto.IsActive;
                permission.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.PermissionRepository.Update(permission);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                {
                    return ApiResponse<bool>.Fail(1, "Failed to update permission.");
                }

                return ApiResponse<bool>.Success(true, "Permission updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error updating permission: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> TogglePermissionStatusAsync(int id)
        {
            try
            {
                var permission = await _unitOfWork.PermissionRepository.FirstOrDefaultAsync(p => p.PermissionId == id);
                if (permission == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Permission with ID {id} not found.");
                }

                permission.IsActive = !permission.IsActive;
                permission.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.PermissionRepository.Update(permission);
                await _unitOfWork.SaveChangesAsync();

                string statusText = permission.IsActive ? "activated" : "deactivated";
                return ApiResponse<bool>.Success(permission.IsActive, $"Permission has been {statusText} successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error updating permission status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeletePermissionAsync(int id)
        {
            try
            {
                var permission = await _unitOfWork.PermissionRepository.FirstOrDefaultAsync(p => p.PermissionId == id);
                if (permission == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Permission with ID {id} not found.");
                }

                // Remove role-permission links first
                var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => rp.PermissionId == id);
                if (rolePermissions.Any())
                {
                    _unitOfWork.RolePermissionRepository.RemoveRange(rolePermissions);
                }

                _unitOfWork.PermissionRepository.Remove(permission);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                {
                    return ApiResponse<bool>.Fail(1, "Failed to delete permission.");
                }

                return ApiResponse<bool>.Success(true, "Permission deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error deleting permission: {ex.Message}");
            }
        }

        #endregion

        #region Role-Permission Mapping

        public async Task<ApiResponse<RolePermissionMappingDto>> GetPermissionsByRoleIdAsync(int roleId)
        {
            try
            {
                var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleId == roleId);
                if (role == null)
                {
                    return ApiResponse<RolePermissionMappingDto>.Fail(1, $"Role with ID {roleId} not found.");
                }

                var allPermissions = await _unitOfWork.PermissionRepository.WhereAsync(p => p.IsActive);
                var assignedRolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => rp.RoleId == roleId);
                var assignedPermissionIds = new HashSet<int>(assignedRolePermissions.Select(rp => rp.PermissionId));

                var permissionItems = allPermissions
                    .OrderBy(p => p.ModuleName)
                    .ThenBy(p => p.PermissionName)
                    .Select(p => new RolePermissionItemDto
                    {
                        PermissionId = p.PermissionId,
                        PermissionCode = p.PermissionCode,
                        PermissionName = p.PermissionName,
                        ModuleName = p.ModuleName,
                        Description = p.Description,
                        IsAssigned = assignedPermissionIds.Contains(p.PermissionId)
                    })
                    .ToList();

                var response = new RolePermissionMappingDto
                {
                    RoleId = role.RoleId,
                    RoleCode = role.RoleCode,
                    RoleName = role.RoleName,
                    Permissions = permissionItems
                };

                return ApiResponse<RolePermissionMappingDto>.Success(response, "Role permissions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<RolePermissionMappingDto>.Fail(500, $"Error retrieving role permissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AssignPermissionsToRoleAsync(AssignRolePermissionsRequestDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<bool>.Fail(1, "Invalid request payload.");

                var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleId == dto.RoleId);
                if (role == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Role with ID {dto.RoleId} not found.");
                }

                // Validate requested permissions
                var requestedPermissionIds = (dto.PermissionIds ?? new List<int>()).Distinct().ToList();
                var validPermissions = await _unitOfWork.PermissionRepository.WhereAsync(
                    p => requestedPermissionIds.Contains(p.PermissionId) && p.IsActive
                );
                var validPermissionIds = validPermissions.Select(p => p.PermissionId).ToHashSet();

                // Fetch existing role permissions
                var existingRolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(
                    rp => rp.RoleId == dto.RoleId
                );

                if (existingRolePermissions.Any())
                {
                    _unitOfWork.RolePermissionRepository.RemoveRange(existingRolePermissions);
                }

                // Add new mappings
                var createdBy = UserId > 0 ? (long?)UserId : null;
                var newRolePermissions = validPermissionIds.Select(pId => new RolePermission
                {
                    RoleId = dto.RoleId,
                    PermissionId = pId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                }).ToList();

                if (newRolePermissions.Any())
                {
                    await _unitOfWork.RolePermissionRepository.AddRangeAsync(newRolePermissions);
                }

                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, $"Permissions mapped to role '{role.RoleName}' successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error assigning permissions to role: {ex.Message}");
            }
        }

        #endregion

        #region User-Role Mapping (Multi-Role Support)

        public async Task<ApiResponse<UserRoleMappingDto>> GetRolesByUserIdAsync(long userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return ApiResponse<UserRoleMappingDto>.Fail(1, $"User with ID {userId} not found.");
                }

                var allRoles = await _unitOfWork.RoleRepository.WhereAsync(r => r.IsActive);
                var userRoles = await _unitOfWork.UserRoleRepository.WhereAsync(ur => ur.UserId == userId);
                var assignedRoleIds = new HashSet<int>(userRoles.Select(ur => ur.RoleId));

                var roleItems = allRoles
                    .OrderBy(r => r.RoleName)
                    .Select(r => new UserRoleItemDto
                    {
                        RoleId = r.RoleId,
                        RoleCode = r.RoleCode,
                        RoleName = r.RoleName,
                        Description = r.Description,
                        IsAssigned = assignedRoleIds.Contains(r.RoleId)
                    })
                    .ToList();

                var response = new UserRoleMappingDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roleItems
                };

                return ApiResponse<UserRoleMappingDto>.Success(response, "User roles retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserRoleMappingDto>.Fail(500, $"Error retrieving user roles: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AssignRolesToUserAsync(AssignUserRolesRequestDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<bool>.Fail(1, "Invalid request payload.");

                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
                if (user == null)
                {
                    return ApiResponse<bool>.Fail(1, $"User with ID {dto.UserId} not found.");
                }

                var requestedRoleIds = (dto.RoleIds ?? new List<int>()).Distinct().ToList();
                var validRoles = await _unitOfWork.RoleRepository.WhereAsync(
                    r => requestedRoleIds.Contains(r.RoleId) && r.IsActive
                );
                var validRoleIds = validRoles.Select(r => r.RoleId).ToHashSet();

                // Fetch existing user roles
                var existingUserRoles = await _unitOfWork.UserRoleRepository.WhereAsync(
                    ur => ur.UserId == dto.UserId
                );

                if (existingUserRoles.Any())
                {
                    _unitOfWork.UserRoleRepository.RemoveRange(existingUserRoles);
                }

                // Add new mappings
                var createdBy = UserId > 0 ? (long?)UserId : null;
                var newUserRoles = validRoleIds.Select(rId => new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = rId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                }).ToList();

                if (newUserRoles.Any())
                {
                    await _unitOfWork.UserRoleRepository.AddRangeAsync(newUserRoles);
                }

                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, $"Roles mapped to user '{user.UserName}' successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error assigning roles to user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserPermissionsResponseDto>> GetUserPermissionsAsync(long userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return ApiResponse<UserPermissionsResponseDto>.Fail(1, $"User with ID {userId} not found.");
                }

                // 1. Get all assigned roles for this user
                var userRoles = await _unitOfWork.UserRoleRepository.WhereAsync(ur => ur.UserId == userId);
                var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();

                var roleNames = new List<string>();
                var permissionsList = new List<PermissionResponseDto>();

                if (roleIds.Any())
                {
                    var roles = await _unitOfWork.RoleRepository.WhereAsync(
                        r => roleIds.Contains(r.RoleId) && r.IsActive
                    );
                    roleNames = roles.Select(r => r.RoleName).ToList();

                    // 2. Get all role-permission mappings for user's assigned roles
                    var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(
                        rp => roleIds.Contains(rp.RoleId)
                    );
                    var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

                    if (permissionIds.Any())
                    {
                        var permissions = await _unitOfWork.PermissionRepository.WhereAsync(
                            p => permissionIds.Contains(p.PermissionId) && p.IsActive
                        );

                        permissionsList = permissions
                            .OrderBy(p => p.ModuleName)
                            .ThenBy(p => p.PermissionName)
                            .Select(p => new PermissionResponseDto
                            {
                                PermissionId = p.PermissionId,
                                PermissionCode = p.PermissionCode,
                                PermissionName = p.PermissionName,
                                ModuleName = p.ModuleName,
                                Description = p.Description,
                                IsActive = p.IsActive,
                                CreatedAt = p.CreatedAt
                            })
                            .ToList();
                    }
                }

                var response = new UserPermissionsResponseDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roleNames,
                    Permissions = permissionsList
                };

                return ApiResponse<UserPermissionsResponseDto>.Success(response, "User aggregated permissions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserPermissionsResponseDto>.Fail(500, $"Error retrieving user permissions: {ex.Message}");
            }
        }

        #endregion
    }
}
