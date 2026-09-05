namespace MediStock360.Application.DTOs.ResponseDto
{
    public class CountryResponseDto
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
