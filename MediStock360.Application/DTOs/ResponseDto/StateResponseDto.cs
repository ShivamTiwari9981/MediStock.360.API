namespace MediStock360.Application.DTOs.ResponseDto
{
    public class StateResponseDto
    {
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public bool? IsActive { get; set; }
    }
}

