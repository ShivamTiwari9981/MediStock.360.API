using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using static MediStock360.Application.Common.GenericProcedureCall;

namespace MediStock360.Application.Services
{
    public class PaymentService : BaseService, IPaymentService
    {
        public PaymentService(IUnitOfWork unitOfWork, ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {

        }

        public async Task<ApiResponse<PaymentResponseDto>> GetPaymentMethod()
        {
            try
            {
                var model = new PaymentResponseDto();
                var param = new List<SqlParameter>{};
                var result = ExecuteStoredProcedure(StoredProcedure.Sp_PaymentMethodes, param, _unitOfWork.GetConnection());
                
                var paymentMethods = CommonMethod.ConvertToList<PaymentMethodDto>(result.Tables[0]);
                var paymentConfigruation = CommonMethod.ConvertToList<PaymentConfigurationDto>(result.Tables[1]);
               
                model.PaymentMethods = paymentMethods;
                model.PaymentConfigurations = paymentConfigruation;
                
                return ApiResponse<PaymentResponseDto>.Success(model);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaymentResponseDto>.Fail(500, $"An error occurred during Get GetPaymentMethod: {ex.Message}");
            }
        }
    }
}
