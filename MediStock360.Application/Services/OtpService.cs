using MediStock360.Application.Common;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Entities;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;


namespace MediStock360.Application.Services
{
    public class OTPService : IOTPService
    {
        private readonly IRedisCacheService _redis;
        private readonly ISettingService _settingService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public OTPService(IUnitOfWork unitOfWork, IRedisCacheService redis,
            ISettingService settingService, IEmailService emailService)
        {
            _redis = redis;
            _settingService = settingService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;

        }

        //public async Task SaveOTP(string userEmail, string otp)
        //{
        //    string key = $"OTP:{userEmail}";

        //    await _redis.SetAsync(
        //        key,
        //        otp,
        //        TimeSpan.FromMinutes(5)
        //    );
        //}

        public async Task SaveOTP(long userId, string userEmail, string otp)
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
                //string otpHash = OtpHelper.HashOtp(otp);

                // Create UserOtp entity
                var userOtp = new UserOtp
                {
                    ClientId = clientId,
                    UserId = userId,
                    //OtpHash = otpHash,
                    OtpHash = otp,
                    OtpType = 1, // 1 for Email OTP
                    AttemptCount = 0,
                    IsUsed = false,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                await _unitOfWork.UserOtpRepository.AddAsync(userOtp);
                await _unitOfWork.SaveChangesAsync();

                // Also save to Redis for quick verification
                string redisKey = $"OTP:{userEmail}";
                try
                {
                    await _redis.SetAsync(
                        redisKey,
                        otp,
                        TimeSpan.FromMinutes(5)
                    );

                    string savedOtp = await _redis.GetAsync(redisKey);
                    if (string.IsNullOrEmpty(savedOtp))
                    {
                        Console.WriteLine("Warning: OTP was not set in Redis cache. Proceeding with database OTP only.");
                    }
                }
                catch (Exception redisEx)
                {
                    Console.WriteLine($"Warning: Failed to save OTP to Redis: {redisEx.Message}. OTP is saved in database only.");
                    // Continue execution - OTP is saved in database which is the fallback
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving OTP: {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<bool>> VerifyOtp(string userEmail, string otp)
        {
            try
            {
                // First, try to verify against Redis for fast verification
                string redisKey = $"OTP:{userEmail}";
                string savedOtp = null;

                try
                {
                    savedOtp = await _redis.GetAsync(redisKey);
                }
                catch (Exception redisEx)
                {
                    Console.WriteLine($"Warning: Failed to access Redis: {redisEx.Message}. Falling back to database verification.");
                }

                // If Redis has the OTP, verify it
                if (!string.IsNullOrWhiteSpace(savedOtp))
                {
                    if (savedOtp != otp)
                    {
                        return ApiResponse<bool>.Fail(1, "Invalid OTP");
                    }

                    // OTP verified successfully, remove from Redis
                    try
                    {
                        await _redis.RemoveAsync(redisKey);
                    }
                    catch (Exception redisEx)
                    {
                        Console.WriteLine($"Warning: Failed to remove OTP from Redis: {redisEx.Message}");
                    }
                }
                else
                {
                    // Redis is empty or unavailable, verify from database
                    var userOtpRecords = await _unitOfWork.UserOtpRepository.WhereAsync(
                        x => x.OtpHash == otp && x.IsUsed == false && x.ExpiresAt > DateTime.UtcNow
                    );

                    if (!userOtpRecords.Any())
                    {
                        return ApiResponse<bool>.Fail(1, "OTP expired or not found");
                    }

                    if (userOtpRecords.First().OtpHash != otp)
                    {
                        return ApiResponse<bool>.Fail(1, "Invalid OTP");
                    }
                }

                // Mark as used in database
                var userOtpRecordsForUpdate = await _unitOfWork.UserOtpRepository.WhereAsync(
                    x => x.OtpHash == otp && x.IsUsed == false
                );

                if (userOtpRecordsForUpdate.Any())
                {
                    var userOtpRecord = userOtpRecordsForUpdate.First();
                    userOtpRecord.IsUsed = true;
                    userOtpRecord.VerifiedAt = DateTime.UtcNow;
                    _unitOfWork.UserOtpRepository.Update(userOtpRecord);
                    await _unitOfWork.SaveChangesAsync();
                }

                return ApiResponse<bool>.Success(true, "OTP verified successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }


        public async Task<ApiResponse<bool>> SendOtpAsync(string userEmail)
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

                // Check if there's an active OTP for this user
                var activeOtp = await _unitOfWork.UserOtpRepository.FirstOrDefaultAsync(
                    x => x.ExpiresAt > DateTime.UtcNow && x.IsUsed == false
                );

                if (activeOtp != null)
                {
                    return ApiResponse<bool>.Fail(1, "An OTP is already active. Please use it or wait for expiry");
                }

                // Generate new OTP
                string otp = OtpHelper.GenerateOtp();

                // Send OTP via email
                //var emailResponse = await _emailService.SendEmailOTP(userEmail, otp);

                //if (!emailResponse.IsSuccess)
                //{
                //    return ApiResponse<bool>.Fail(1, emailResponse.Message);
                //}

                // Save OTP to database and Redis
                await SaveOTP(user.UserId, userEmail, otp);

                return ApiResponse<bool>.Success(true, "OTP sent successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> VerifyEmailOTP(string userEmail, string otp)
        {
            try
            {
                var result = await VerifyOtp(userEmail, otp);
                return result;
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }


    }
}
