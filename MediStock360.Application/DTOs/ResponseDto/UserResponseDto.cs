namespace MediStock360.Application.DTOs.ResponseDto
{
    public class UserResponseDto
    {
        public long UserId { get; set; }
        public Guid UserKey { get; set; }
        public long ClientId { get; set; }
        public string ProfileImageUrl { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsEmailVarified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
    }
}
