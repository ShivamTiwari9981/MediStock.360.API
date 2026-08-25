using MediStock360.Application.Common.constaints;
using MediStock360.Application.Interfaces;
using MediStock360.Application.Services;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure.Interfaces;


namespace HRMS.Application.Services
{
    public class SettingService : BaseService, ISettingService
    {

        private const string EMAIL_OTP_KEY = SettingKeys.EnableEmailOtp;
        private readonly IRedisCacheService _cache;
        public SettingService(IUnitOfWork unitOfWork, IRedisCacheService cache, ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {
            _cache = cache;
        }

        public async Task<bool> IsEmailOtpEnabled()
        {
            // 1. Check Cache
            var cachedValue = await _cache.GetAsync(EMAIL_OTP_KEY);
            bool boolValue = Convert.ToBoolean(cachedValue);
            if (boolValue)
                return true;

            // 2. DB Hit
            var setting = "";
            //var setting = await _unitOfWork.HRMSAppSettingRepository.FirstOrDefaultAsync(x => x.SettingKey == EMAIL_OTP_KEY && x.ClientId == ClientId);

            if (setting == null)
                return false;

            // 3. Store in Cache
            //await _cache.SetAsync(EMAIL_OTP_KEY, setting.SettingValue, DateHelper.GetTimeSpan(5));

            return false;
        }

    }
}

