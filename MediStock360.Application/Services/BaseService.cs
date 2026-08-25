using System;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;

namespace MediStock360.Application.Services
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentUserService _currentUserService;

        protected BaseService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        protected long ClientId => _currentUserService.ClientId;
        protected Guid ClientKey => _currentUserService.ClientKey;
        protected long UserId => _currentUserService.UserId;
        protected int RoleId => _currentUserService.RoleId;
        protected long StoreId => _currentUserService.StoreId;
        protected Guid StoreKey => _currentUserService.StoreKey;
    }
}

