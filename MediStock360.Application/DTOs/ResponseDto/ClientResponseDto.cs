namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ClientResponseDto
    {
        public int ClientId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
