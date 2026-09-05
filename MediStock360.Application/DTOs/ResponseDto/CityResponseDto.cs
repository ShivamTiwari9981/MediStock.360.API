namespace MediStock360.Application.DTOs.ResponseDto
{
    public class CityResponseDto
    {
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public string CityName { get; set; }
        public bool ? IsActive { get; set; }
    }
}
