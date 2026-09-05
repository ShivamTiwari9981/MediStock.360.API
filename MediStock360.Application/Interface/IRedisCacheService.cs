namespace MediStock360.Application.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetAsync(
            string key,
            string value,
            TimeSpan expiry);

        Task<string> GetAsync(string key);

        Task RemoveAsync(string key);
    }
}
