using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class CreateMenuRequestDto
    {
        public int? ParentMenuId { get; set; }

        [Required(ErrorMessage = "Menu Name is required")]
        [MaxLength(200, ErrorMessage = "Menu Name cannot exceed 200 characters")]
        public string MenuName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Menu Icon is required")]
        [MaxLength(50, ErrorMessage = "Menu Icon cannot exceed 50 characters")]
        public string MenuIcon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Router Link is required")]
        [MaxLength(100, ErrorMessage = "Router Link cannot exceed 100 characters")]
        public string RouterLink { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Permission Code cannot exceed 100 characters")]
        public string? PermissionCode { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsVisible { get; set; } = true;

        public int IsActive { get; set; } = 1;
    }

    public class UpdateMenuRequestDto
    {
        public int? ParentMenuId { get; set; }

        [Required(ErrorMessage = "Menu Name is required")]
        [MaxLength(200, ErrorMessage = "Menu Name cannot exceed 200 characters")]
        public string MenuName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Menu Icon is required")]
        [MaxLength(50, ErrorMessage = "Menu Icon cannot exceed 50 characters")]
        public string MenuIcon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Router Link is required")]
        [MaxLength(100, ErrorMessage = "Router Link cannot exceed 100 characters")]
        public string RouterLink { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Permission Code cannot exceed 100 characters")]
        public string? PermissionCode { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsVisible { get; set; } = true;

        public int IsActive { get; set; } = 1;
    }
}

