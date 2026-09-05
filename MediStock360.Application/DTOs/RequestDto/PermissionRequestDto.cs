using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class CreatePermissionRequestDto
    {
        [Required(ErrorMessage = "Permission Code is required")]
        [MaxLength(100, ErrorMessage = "Permission Code cannot exceed 100 characters")]
        public string PermissionCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Permission Name is required")]
        [MaxLength(150, ErrorMessage = "Permission Name cannot exceed 150 characters")]
        public string PermissionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Module Name is required")]
        [MaxLength(100, ErrorMessage = "Module Name cannot exceed 100 characters")]
        public string ModuleName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdatePermissionRequestDto
    {
        [Required(ErrorMessage = "Permission Name is required")]
        [MaxLength(150, ErrorMessage = "Permission Name cannot exceed 150 characters")]
        public string PermissionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Module Name is required")]
        [MaxLength(100, ErrorMessage = "Module Name cannot exceed 100 characters")]
        public string ModuleName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AssignRolePermissionsRequestDto
    {
        [Required(ErrorMessage = "Role ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Role ID is required")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Permission IDs list is required")]
        public List<int> PermissionIds { get; set; } = new();
    }

    public class AssignUserRolesRequestDto
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, long.MaxValue, ErrorMessage = "A valid User ID is required")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "Role IDs list is required")]
        public List<int> RoleIds { get; set; } = new();
    }
}

