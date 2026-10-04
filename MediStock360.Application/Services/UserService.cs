using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ICurrentUserService _currentService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, ICurrentUserService currentService
            )
        {
            _currentService = currentService;
            _unitOfWork = unitOfWork;

        }

        public async Task<ApiResponse<bool>> IsUserExist(string userEmail)
        {
            try
            {

                // Check if user exists with this email
                var user = await _unitOfWork.UserRepository.AnyAsync(
                    u => u.Email == userEmail
                );

                if (!user)
                {
                    return ApiResponse<bool>.Fail(1, "User not found with this email");
                }

                return ApiResponse<bool>.Success(user);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }
    }
}
