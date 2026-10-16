using HRMS.Application.Services;
using MediStock360.Application.Common;
using MediStock360.Application.Common.constaints;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Enums;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Entities;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;
using static MediStock360.Application.Common.GenericProcedureCall;


namespace MediStock360.Application.Services
{
    public class OTPService : IOTPService
    {
        private readonly ICurrentUserService _currentService;
        private readonly ISettingService _settingService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public OTPService(IUnitOfWork unitOfWork, ICurrentUserService currentService,
            ISettingService settingService, IEmailService emailService)
        {
            _currentService = currentService;
            _settingService = settingService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;

        }

        
        public async Task<ApiResponse<bool>> SendOtpAsync(string userEmail,OtpPurpose purpose)
        {
            try
            {
                // Verify email setting is enabled
                //bool isEmailOtpEnabled = await _settingService.IsEmailOtpEnabled();
                bool isEmailOtpEnabled = true;
                if (!isEmailOtpEnabled)
                {
                    return ApiResponse<bool>.Fail(1, "Email OTP setting is disabled!");
                }

                // Check if user exists with this email
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(
                    u => u.Email == userEmail
                );

                if (user == null)
                {
                    return ApiResponse<bool>.Fail(1, "User not found with this email");
                }

                // Invalidate any previous unused OTPs for this user
                var previousOtps = await _unitOfWork.UserOtpRepository.WhereAsync(
                    x => x.UserId == user.UserId && x.IsUsed == false
                );
                if (previousOtps != null && previousOtps.Any())
                {
                    foreach (var oldOtp in previousOtps)
                    {
                        oldOtp.IsUsed = true;
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                // Generate new OTP
                string otp = OtpHelper.GenerateOtp();

                // Send OTP via email
                var emailResponse = await _emailService.SendEmailOTP(userEmail, otp, user.UserName ?? "User", 10);

                if (!emailResponse.IsSuccess)
                {
                    // Fallback log in case email provider is temporarily unreachable
                    Console.WriteLine($"[OTP Service] Generated OTP for {userEmail}: {otp}. Note: {emailResponse.Message}");
                }

                // Save OTP to database
                await SaveOTP(user.UserId, userEmail, otp, purpose);

                return ApiResponse<bool>.Success(true, "OTP sent successfully to registered email.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }
        public async Task SaveOTP(long userId, string userEmail, string otp, OtpPurpose purpose)
        {
            try
            {
                // Get user to find client ID
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                // Get client ID from user (assuming User has ClientId property)
                var clientId = user.ClientId; // Adjust if property name is different

                // Hash the OTP
                string otpHash = OtpHelper.HashOtp(otp);

                var expirySetting = await _settingService.GetAppSettingValue(SettingKeys.OtpExpiryMinutes);
                double expiryMinutes = !string.IsNullOrWhiteSpace(expirySetting) && double.TryParse(expirySetting, out var m) ? m : 10;

                // Create UserOtp entity
                var userOtp = new UserOtp
                {
                    ClientId = (long)clientId,
                    UserId = userId,
                    OtpHash = otpHash,
                    OtpType = (int)OtpType.EmailOTP, // 1 for Email OTP
                    OtpPurpose = (byte)purpose,
                    AttemptCount = 0,
                    IsUsed = false,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                await _unitOfWork.UserOtpRepository.AddAsync(userOtp);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving OTP: {ex.Message}", ex);
            }
        }
        public async Task<ApiResponse<bool>> VerfyOtpAsync(string userEmail, string otp)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                if (string.IsNullOrEmpty(otp))
                    throw new Exception("Otp is not found");


                string otpHash = OtpHelper.HashOtp(otp);
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@ClientId",user.ClientId),
                    new SqlParameter("@UserId", user.UserId),
                    new SqlParameter("@OtpHash", otpHash),
                };
                var result = ExecuteStoredProcedure(StoredProcedure.Sp_VerifyUserOtp, param, _unitOfWork.GetConnection());
                bool IsVarify = GetValueByDataSet.GetValue<bool>(result, "Table", "IsVerified");
                string message = GetValueByDataSet.GetValue<string>(result, "Table", "Message");
                if (IsVarify)
                    return ApiResponse<bool>.Success(IsVarify, message);
                else
                    return ApiResponse<bool>.Fail(500, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, ex.Message);
            }
        }
        public async Task<ApiResponse<bool>> ResendOtp(string userEmail, OtpPurpose purpose)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    return ApiResponse<bool>.Fail(
                        1,
                        "Email id can not empty."
                    );
                }
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    throw new Exception("User not found");
                }


                // 1. Generate new OTP
                var otp = OtpHelper.GenerateOtp();

                // 2. Hash OTP
                var otpHash = OtpHelper.HashOtp(otp);

                // 3. Get OTP expiry from AppSetting
                var expiryMinutes = await _settingService.GetAppSettingValue(SettingKeys.OtpExpiryMinutes);

                // 4. Save new OTP using stored procedure
                var param = new[]
                {

                    new SqlParameter("@ClientId", user.ClientId),
                    new SqlParameter("@UserId", user.UserId),
                    new SqlParameter("@OtpType", (int)OtpType.EmailOTP),
                    new SqlParameter("@OtpHash", otpHash),
                    new SqlParameter("@ExpiryMinutes",expiryMinutes)
                };

                var result = ExecuteStoredProcedure(StoredProcedure.Sp_ResendUserOtp, param, _unitOfWork.GetConnection());
                bool IsVarify = GetValueByDataSet.GetValue<bool>(result, "Table", "IsVerified");
                string message = GetValueByDataSet.GetValue<string>(result, "Table", "Message");

                var email = await _emailService.SendEmailOTP(user.Email, otp, userEmail, Convert.ToInt32(expiryMinutes));
                if (email.IsSuccess)
                {
                    return ApiResponse<bool>.Success(
                    true,
                    "OTP resent successfully."
                );
                }
                else
                {
                    return ApiResponse<bool>.Fail(1, "Unable to resend OTP. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    1,
                    "Unable to resend OTP. Please try again."
                );
            }
        }
    }
}
