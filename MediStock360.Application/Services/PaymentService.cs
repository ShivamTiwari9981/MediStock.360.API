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
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly ITelegramBotService _telegramBotService;

        public PaymentService(IUnitOfWork unitOfWork, ICurrentUserService currentSession, ITelegramBotService telegramBotService)
            : base(unitOfWork, currentSession)
        {
            _telegramBotService = telegramBotService;
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

        public async Task<ApiResponse<ClientPaymentResponseDto>> SubmitClientPaymentAsync(SaveClientPaymentRequestDto dto)
        {
            try
            {
                long effectiveClientId = ClientId != 0 ? ClientId : (dto.ClientId ?? 0);
                if (effectiveClientId == 0)
                {
                    return ApiResponse<ClientPaymentResponseDto>.Fail(400, "Client ID could not be identified.");
                }

                if (string.IsNullOrWhiteSpace(dto.TransactionRefNo))
                {
                    return ApiResponse<ClientPaymentResponseDto>.Fail(400, "Transaction reference or UTR number is required.");
                }

                // 1. Fetch Client info
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == effectiveClientId);

                // 2. Resolve ClientSubscription
                ClientSubscription? subscription = null;
                if (dto.ClientSubscriptionId.HasValue && dto.ClientSubscriptionId.Value > 0)
                {
                    subscription = await _unitOfWork.ClientSubscriptionRepository.FirstOrDefaultAsync(
                        s => s.ClientSubscriptionId == dto.ClientSubscriptionId.Value
                    );
                }

                if (subscription == null)
                {
                    subscription = await _unitOfWork.ClientSubscriptionRepository.FirstOrDefaultAsync(
                        s => s.ClientId == effectiveClientId && (s.Status == 1 || s.Status == 2)
                    );
                }

                // 3. Resolve PaymentConfigurationId
                long paymentConfigId = dto.PaymentConfigurationId ?? 0;
                if (paymentConfigId == 0)
                {
                    string modeCode = (dto.PaymentMode ?? "UPI").ToUpper();
                    var allConfigs = await _unitOfWork.PaymentConfigurationRepository.WhereAsync(c => c.IsActive);
                    var matchedConfig = allConfigs.FirstOrDefault(c => (c.ConfigurationCode ?? "").ToUpper().Contains(modeCode))
                                      ?? allConfigs.FirstOrDefault();
                    paymentConfigId = matchedConfig?.PaymentConfigurationId ?? 1;
                }

                // 4. Save to ClientPayment table (Status = 1 Pending - DO NOT ACTIVATE UNTIL ADMIN APPROVES)
                var clientPayment = new ClientPayment
                {
                    ClientId = effectiveClientId,
                    ClientSubscriptionId = subscription?.ClientSubscriptionId ?? 0,
                    PaymentConfigurationId = paymentConfigId,
                    Amount = dto.Amount,
                    CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "INR" : dto.CurrencyCode,
                    TransactionReference = dto.TransactionRefNo.Trim(),
                    SenderNotes = dto.PaymentNotes?.Trim(),
                    PaymentStatus = 1, // 1 = Pending Approval
                    SubmittedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };

                await _unitOfWork.ClientPaymentRepository.AddAsync(clientPayment);

                // 5. Update subscription to Pending Payment status (Do not activate until admin approves)
                if (subscription != null)
                {
                    subscription.PaymentStatus = 1; // 1 = Pending
                    subscription.Status = 1;        // 1 = Pending/Trial (NOT 2 Active)
                    subscription.TransactionReference = dto.TransactionRefNo.Trim();
                    subscription.UpdatedAt = DateTime.UtcNow;
                    subscription.UpdatedBy = UserId != 0 ? UserId : null;
                    _unitOfWork.ClientSubscriptionRepository.Update(subscription);
                }

                // 6. Ensure Client remains inactive until admin approves
                if (client != null)
                {
                    client.IsActive = false; // Must be approved by Admin
                    client.UpdatedAt = DateTime.UtcNow;
                    client.UpdatedBy = UserId != 0 ? UserId : null;
                    _unitOfWork.ClientRepository.Update(client);
                }

                await _unitOfWork.SaveChangesAsync();

                // 7. Resolve plan name for Telegram notification
                string planName = "Subscription Plan";
                if (subscription != null && subscription.SubscriptionPlanId > 0)
                {
                    var plan = await _unitOfWork.SubscriptionPlanRepository.FirstOrDefaultAsync(p => p.SubscriptionPlanId == subscription.SubscriptionPlanId);
                    if (plan != null)
                    {
                        planName = plan.PlanName;
                    }
                }

                // 8. Fire-and-forget Telegram notification to Admin
                var notification = new ClientPaymentNotificationDto
                {
                    ClientPaymentId = clientPayment.ClientPaymentId,
                    ClientId = effectiveClientId,
                    ClientName = client?.ClientName ?? "Client",
                    CompanyName = client?.CompanyName ?? "Company",
                    PlanName = planName,
                    Amount = clientPayment.Amount,
                    CurrencyCode = clientPayment.CurrencyCode,
                    PaymentMode = dto.PaymentMode ?? "UPI",
                    TransactionReference = clientPayment.TransactionReference,
                    SenderNotes = clientPayment.SenderNotes,
                    SubmittedAt = clientPayment.SubmittedAt
                };

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _telegramBotService.SendPaymentNotificationAsync(notification);
                    }
                    catch
                    {
                        // Ignore notification failures so payment process remains intact
                    }
                });

                var responseDto = new ClientPaymentResponseDto
                {
                    ClientPaymentId = clientPayment.ClientPaymentId,
                    ClientId = clientPayment.ClientId,
                    ClientSubscriptionId = clientPayment.ClientSubscriptionId,
                    PaymentConfigurationId = clientPayment.PaymentConfigurationId,
                    Amount = clientPayment.Amount,
                    CurrencyCode = clientPayment.CurrencyCode,
                    TransactionReference = clientPayment.TransactionReference,
                    SenderNotes = clientPayment.SenderNotes,
                    PaymentStatus = clientPayment.PaymentStatus,
                    SubmittedAt = clientPayment.SubmittedAt
                };

                return ApiResponse<ClientPaymentResponseDto>.Success(
                    responseDto,
                    "Payment reference submitted successfully. Your account will be activated once verified and approved by Administrator."
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ClientPaymentResponseDto>.Fail(500, $"An error occurred while submitting payment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ClientPaymentResponseDto>> ApproveClientPaymentAsync(ApprovePaymentRequestDto dto)
        {
            try
            {
                var payment = await _unitOfWork.ClientPaymentRepository.FirstOrDefaultAsync(p => p.ClientPaymentId == dto.ClientPaymentId);
                if (payment == null)
                {
                    return ApiResponse<ClientPaymentResponseDto>.Fail(404, $"Payment record with ID {dto.ClientPaymentId} not found.");
                }

                byte newStatus = (byte)(dto.IsApproved ? 2 : 3); // 2 = Approved, 3 = Rejected
                payment.PaymentStatus = newStatus;
                payment.AdminRemarks = dto.AdminRemarks;
                payment.ReviewedBy = UserId != 0 ? UserId : null;
                payment.ReviewedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = UserId != 0 ? UserId : null;
                _unitOfWork.ClientPaymentRepository.Update(payment);

                var subscription = await _unitOfWork.ClientSubscriptionRepository.FirstOrDefaultAsync(s => s.ClientSubscriptionId == payment.ClientSubscriptionId);
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == payment.ClientId);

                string planName = "Subscription Plan";
                if (subscription != null)
                {
                    var plan = await _unitOfWork.SubscriptionPlanRepository.FirstOrDefaultAsync(p => p.SubscriptionPlanId == subscription.SubscriptionPlanId);
                    if (plan != null) planName = plan.PlanName;

                    if (dto.IsApproved)
                    {
                        subscription.PaymentStatus = 2; // Paid
                        subscription.Status = 2;        // Active
                        subscription.StartDate = DateTime.UtcNow;
                        subscription.EndDate = subscription.BillingCycle == 2 ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMonths(1);
                    }
                    else
                    {
                        subscription.PaymentStatus = 3; // Failed / Rejected
                    }
                    subscription.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.ClientSubscriptionRepository.Update(subscription);
                }

                if (client != null)
                {
                    if (dto.IsApproved)
                    {
                        client.IsActive = true; // ACTIVATED BY ADMIN!
                        client.IsOnboardingCompleted = true;
                        client.OnboardingStep = 4;
                    }
                    client.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.ClientRepository.Update(client);
                }

                await _unitOfWork.SaveChangesAsync();

                // Send Telegram approval notification
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _telegramBotService.SendPaymentApprovalNotificationAsync(
                            client?.ClientName ?? "Client",
                            client?.CompanyName ?? "Company",
                            planName,
                            payment.Amount,
                            dto.IsApproved,
                            dto.AdminRemarks
                        );
                    }
                    catch
                    {
                        // Ignore notification failures
                    }
                });

                var responseDto = new ClientPaymentResponseDto
                {
                    ClientPaymentId = payment.ClientPaymentId,
                    ClientId = payment.ClientId,
                    ClientSubscriptionId = payment.ClientSubscriptionId,
                    PaymentConfigurationId = payment.PaymentConfigurationId,
                    Amount = payment.Amount,
                    CurrencyCode = payment.CurrencyCode,
                    TransactionReference = payment.TransactionReference,
                    SenderNotes = payment.SenderNotes,
                    PaymentStatus = payment.PaymentStatus,
                    AdminRemarks = payment.AdminRemarks,
                    ReviewedBy = payment.ReviewedBy,
                    ReviewedAt = payment.ReviewedAt,
                    SubmittedAt = payment.SubmittedAt
                };

                string msg = dto.IsApproved
                    ? "Payment approved successfully. Client subscription is now active."
                    : "Payment was rejected. Client remains inactive.";

                return ApiResponse<ClientPaymentResponseDto>.Success(responseDto, msg);
            }
            catch (Exception ex)
            {
                return ApiResponse<ClientPaymentResponseDto>.Fail(500, $"An error occurred during payment review: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> TestTelegramBotAsync(string testMessage)
        {
            try
            {
                string msg = string.IsNullOrWhiteSpace(testMessage)
                    ? "🔔 <b>MediStock360 Telegram Bot Test Notification</b>\n\nTelegram Bot is connected and operating normally!"
                    : $"🔔 <b>MediStock360 Test Message</b>\n\n{testMessage}";

                bool success = await _telegramBotService.SendMessageAsync(msg);
                if (success)
                {
                    return ApiResponse<bool>.Success(true, "Test message sent successfully to Telegram.");
                }
                return ApiResponse<bool>.Fail(400, "Could not send Telegram message. Please check BotToken, ChatId and IsEnabled in configuration.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Telegram test failed: {ex.Message}");
            }
        }
    }
}
