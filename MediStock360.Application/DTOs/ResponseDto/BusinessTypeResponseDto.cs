namespace MediStock360.Application.DTOs.ResponseDto
{
    public class BusinessTypeResponseDto
    {
        public int BusinessTypeId { get; set; }
        public string BusinessTypeCode { get; set; }
        public string BusinessTypeName { get; set; }
        public bool? IsActive { get; set; }
    }
}
