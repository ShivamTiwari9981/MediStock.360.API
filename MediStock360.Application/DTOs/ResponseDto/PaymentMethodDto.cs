namespace MediStock360.Application.DTOs.ResponseDto
{
        public class PaymentMethodDto
        {
            public int PaymentMethodId { get; set; }
            public string MethodCode { get; set; } = string.Empty;
            public string MethodName { get; set; } = string.Empty;
            public string? Description { get; set; }

            public bool IsActive { get; set; } = true;

            public int DisplayOrder { get; set; }
        }
    }
