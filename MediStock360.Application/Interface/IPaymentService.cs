using MediStock360.Application.DTOs.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.Interface
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDto>> GetPaymentMethod();
    }
}
