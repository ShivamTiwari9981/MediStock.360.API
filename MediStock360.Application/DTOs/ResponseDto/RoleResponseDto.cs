

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class RoleResponseDto
    {
        public int RoleId { get; set; }

        public long? ClientId { get; set; }

        public string RoleCode { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsSystemRole { get; set; }

        public bool IsActive { get; set; }
    }

   
}
