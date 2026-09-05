using System;
using System.Collections.Generic;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class PermissionResponseDto
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ModuleGroupedPermissionsDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public int TotalPermissions { get; set; }
        public List<PermissionResponseDto> Permissions { get; set; } = new();
    }

    public class RolePermissionItemDto
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class RolePermissionMappingDto
    {
        public int RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<RolePermissionItemDto> Permissions { get; set; } = new();
    }

    public class UserRoleItemDto
    {
        public int RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class UserRoleMappingDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public List<UserRoleItemDto> Roles { get; set; } = new();
    }

    public class UserPermissionsResponseDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<PermissionResponseDto> Permissions { get; set; } = new();
    }
}

