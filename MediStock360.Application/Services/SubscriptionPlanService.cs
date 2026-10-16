using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static MediStock360.Application.Common.GenericProcedureCall;

namespace MediStock360.Application.Services
{
    public class SubscriptionPlanService : BaseService, ISubscriptionPlanService
    {
        public SubscriptionPlanService(IUnitOfWork unitOfWork, ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {

        }
        
        public ApiResponse<List<SubscriptionPlanDto>>GetSubscriptionPlan()
        {
            try
            {
                
                var param = new List<SqlParameter> {};
                var result = ExecuteStoredProcedureDataSetAsync(StoredProcedure.Sp_GetSubscriptionPlans, param, _unitOfWork.GetConnection());
                var dbResult = CommonMethod.ConvertToList<SubscriptionPlanResponseDto>(result.Result.Tables[0]);

                var groupedResult = GroupSubscriptionFeature(dbResult);


                return ApiResponse<List<SubscriptionPlanDto>>.Success(groupedResult);

                //return ApiResponse<LoginResponseDto>.Success(null, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SubscriptionPlanDto>>.Fail(500, $"An error occurred during Update Client: {ex.Message}");
            }
        }

        private List<SubscriptionPlanDto> GroupSubscriptionFeature(List<SubscriptionPlanResponseDto> dbResult)
        {
                var groupedPlans = dbResult
       .GroupBy(x => x.SubscriptionPlanId)
       .Select(g =>
       {
           var plan = g.First();

           return new SubscriptionPlanDto
           {
               SubscriptionPlanId = plan.SubscriptionPlanId,
               PlanCode = plan.PlanCode,
               PlanName = plan.PlanName,
               Description = plan.Description,
               BillingCycle = plan.BillingCycle,
               Price = plan.Price,
               CurrencyCode = plan.CurrencyCode,

               Features = g
                   .OrderBy(x => x.DisplayOrder)
                   .Select(x => new SubscriptionFeatureDto
                   {
                       SubscriptionFeatureId =
                           x.SubscriptionFeatureId,

                       FeatureCode = x.FeatureCode,
                       FeatureName = x.FeatureName,
                       FeatureValue = x.FeatureValue,
                       IsEnabled = x.IsEnabled,
                       DisplayOrder = x.DisplayOrder,
                       DisplayTemplate = x.DisplayTemplate
                   })
                   .ToList()
           };
       })
       .OrderBy(x => x.SubscriptionPlanId)
       .ToList();

            return groupedPlans;
        }

        public async Task<ApiResponse<ConfirmSubscriptionPlanResponseDto>> ConfirmSubscriptionPlanAsync(ConfirmSubscriptionPlanRequestDto dto)
        {
            try
            {
                long effectiveClientId = ClientId != 0 ? ClientId : (dto.ClientId ?? 0);
                if (effectiveClientId == 0)
                {
                    return ApiResponse<ConfirmSubscriptionPlanResponseDto>.Fail(400, "Client ID could not be identified for confirmation.");
                }

                // Verify subscription plan exists
                var plan = await _unitOfWork.SubscriptionPlanRepository.FirstOrDefaultAsync(p => p.SubscriptionPlanId == dto.SubscriptionPlanId);
                if (plan == null)
                {
                    return ApiResponse<ConfirmSubscriptionPlanResponseDto>.Fail(404, $"Subscription plan with ID {dto.SubscriptionPlanId} not found.");
                }

                byte billingCycle = dto.BillingCycle ?? (byte)plan.BillingCycle;
                decimal amount = dto.Price ?? plan.Price;
                bool isFreePlan = amount == 0;

                // Check for existing active or pending subscription for this client
                var existingSub = await _unitOfWork.ClientSubscriptionRepository.FirstOrDefaultAsync(
                    s => s.ClientId == effectiveClientId && (s.Status == 1 || s.Status == 2)
                );

                ClientSubscription subscription;
                if (existingSub != null)
                {
                    existingSub.SubscriptionPlanId = plan.SubscriptionPlanId;
                    existingSub.BillingCycle = billingCycle;
                    existingSub.Amount = amount;
                    existingSub.CurrencyCode = plan.CurrencyCode ?? "INR";
                    existingSub.Status = (byte)(isFreePlan ? 2 : 1); // 2 = Active, 1 = Trial / Pending Payment
                    existingSub.PaymentStatus = (byte)(isFreePlan ? 2 : 1); // 2 = Paid, 1 = Pending
                    existingSub.IsTrial = isFreePlan;
                    existingSub.StartDate = DateTime.UtcNow;
                    existingSub.EndDate = billingCycle == 2 ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMonths(1);
                    existingSub.UpdatedAt = DateTime.UtcNow;
                    existingSub.UpdatedBy = UserId;

                    _unitOfWork.ClientSubscriptionRepository.Update(existingSub);
                    subscription = existingSub;
                }
                else
                {
                    subscription = new ClientSubscription
                    {
                        ClientId = effectiveClientId,
                        SubscriptionPlanId = plan.SubscriptionPlanId,
                        BillingCycle = billingCycle,
                        Amount = amount,
                        CurrencyCode = plan.CurrencyCode ?? "INR",
                        Status = (byte)(isFreePlan ? 2 : 1), // 2 = Active, 1 = Trial / Pending Payment
                        PaymentStatus = (byte)(isFreePlan ? 2 : 1), // 2 = Paid, 1 = Pending
                        IsTrial = isFreePlan,
                        StartDate = DateTime.UtcNow,
                        EndDate = billingCycle == 2 ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMonths(1),
                        AutoRenew = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = UserId
                    };

                    await _unitOfWork.ClientSubscriptionRepository.AddAsync(subscription);
                }

                await _unitOfWork.SaveChangesAsync();

                var response = new ConfirmSubscriptionPlanResponseDto
                {
                    ClientSubscriptionId = subscription.ClientSubscriptionId,
                    ClientId = subscription.ClientId,
                    SubscriptionPlanId = subscription.SubscriptionPlanId,
                    PlanName = plan.PlanName,
                    PlanCode = plan.PlanCode,
                    BillingCycle = subscription.BillingCycle,
                    Amount = subscription.Amount,
                    CurrencyCode = subscription.CurrencyCode,
                    Status = subscription.Status,
                    PaymentStatus = subscription.PaymentStatus,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    RequiresPayment = !isFreePlan,
                    Message = isFreePlan
                        ? "Free plan activated successfully."
                        : "Plan confirmed successfully. Please proceed with payment."
                };

                return ApiResponse<ConfirmSubscriptionPlanResponseDto>.Success(response, response.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<ConfirmSubscriptionPlanResponseDto>.Fail(500, $"An error occurred while confirming subscription plan: {ex.Message}");
            }
        }
    }
}
