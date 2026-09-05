using System.Collections.Generic;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class MenuResponseDto
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public string? ParentMenuName { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuIcon { get; set; } = string.Empty;
        public string RouterLink { get; set; } = string.Empty;
        public string? PermissionCode { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; }
        public List<MenuResponseDto> SubMenus { get; set; } = new();
    }
}

