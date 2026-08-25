using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using static MediStock360.Application.Common.Global;

namespace MediStock360.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long ClientId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst("ClientId")?.Value;

                return string.IsNullOrEmpty(value)
                    ? 0
                    : long.Parse(value);
            }
        }

        public long UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(Claim_Types.UserId)?.Value;

                return string.IsNullOrEmpty(value)
                    ? 0
                    : long.Parse(value);
            }
        }

        public int RoleId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(Claim_Types.RoleIdKey)?.Value;

                return string.IsNullOrEmpty(value)
                   ? 0
                   : int.Parse(value);
            }
        }

        public Guid ClientKey
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(Claim_Types.ClientKey)?.Value;

                return string.IsNullOrEmpty(value)
                    ? Guid.Empty
                    : Guid.Parse(value);
            }
        }

        public long StoreId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(Claim_Types.StoreId)?.Value;

                return string.IsNullOrEmpty(value)
                    ? 0
                    : long.Parse(value);
            }
        }

        public Guid StoreKey
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(Claim_Types.StoreKey)?.Value;

                return string.IsNullOrEmpty(value)
                    ? Guid.Empty
                    : Guid.Parse(value);
            }
        }

        //public int RoleId
        //    {
        //        get
        //        {
        //            var value = _httpContextAccessor.HttpContext?
        //                .User?
        //                .FindFirst(Claim_Types.RoleIdKey)?.Value;

        //            return Convert.ToInt32(value)
        //                ? 0
        //                : value;
        //        }
        //    }
        //}
    }
}
