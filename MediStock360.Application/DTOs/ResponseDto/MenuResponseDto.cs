using Newtonsoft.Json;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class MenuResponseDto
    {
        public int ParrentMenuId { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public string RouterLink { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string SubMenus { get; set; }
        public List<SubMenuResponseDto> SubMenu
        {
            get
            {
                if (string.IsNullOrEmpty(SubMenus))
                    return new List<SubMenuResponseDto>();

                return JsonConvert.DeserializeObject<List<SubMenuResponseDto>>(SubMenus);
            }
        }
    }

    public class SubMenuResponseDto
    {
        public Guid ParrentMenuId { get; set; }
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public string RouterLink { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }
        //public MenuType MenuType { get; set; }
        public bool IsActive { get; set; }
        
    }
}
