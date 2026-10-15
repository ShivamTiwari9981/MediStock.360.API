namespace MediStock360.Application.DTOs.ResponseDto
{
    public class PaymentResponseDto
    {
        public List<PaymentConfigurationDto>PaymentConfigurations { get; set; }
        public List<PaymentMethodDto> PaymentMethods { get; set; }
    }
}
