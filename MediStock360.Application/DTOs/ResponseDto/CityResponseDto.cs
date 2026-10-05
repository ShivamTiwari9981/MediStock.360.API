namespace MediStock360.Application.DTOs.ResponseDto
{
    public class CityResponseDto
    {
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string CityName { get; set; }
        public bool? IsActive { get; set; }
    }
}

