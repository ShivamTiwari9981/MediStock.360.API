namespace MediStock360.Application.Interfaces
{
    public interface ICurrentUserService
    {
        long UserId { get; }
        long ClientId { get; }
        Guid ClientKey { get; }
        long StoreId { get; }
        Guid StoreKey { get; }
        int RoleId { get; }
    }
}
