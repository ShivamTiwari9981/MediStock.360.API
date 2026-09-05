namespace MediStock360.Application.Interfaces
{
    public interface ISettingService
    {
        Task<bool> IsEmailOtpEnabled();
        Task<string> GetAppSettingValue(string settingKey);
    }
}
