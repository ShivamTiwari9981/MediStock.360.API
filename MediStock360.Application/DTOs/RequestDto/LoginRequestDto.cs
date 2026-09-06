using System.ComponentModel.DataAnnotations;

namespace MediStock360.Application.DTOs.RequestDto
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email or Username is required")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class SwitchClientRequestDto
    {
        [Required(ErrorMessage = "Client ID is required")]
        [Range(1, long.MaxValue, ErrorMessage = "A valid Client ID is required")]
        public long ClientId { get; set; }

        public long? StoreId { get; set; }
    }

    public class SwitchStoreRequestDto
    {
        [Required(ErrorMessage = "Store ID is required")]
        [Range(1, long.MaxValue, ErrorMessage = "A valid Store ID is required")]
        public long StoreId { get; set; }
    }
}
