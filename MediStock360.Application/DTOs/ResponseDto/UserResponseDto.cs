namespace MediStock360.Application.DTOs.ResponseDto
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsCompanyProfileCreated { get; set; }
    }
}
