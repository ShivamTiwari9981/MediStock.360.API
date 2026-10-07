using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;
using static MediStock360.Application.Common.GenericProcedureCall;

namespace MediStock360.Application.Services
{
    public class StoreService :  BaseService,IStoreService
    {
        public StoreService(IUnitOfWork unitOfWork, ICurrentUserService currentSession)
            : base(unitOfWork, currentSession)
        {

        }

        public ApiResponse<StoreResponseDto> AddStore (StoreRequestDto dto)
        {
            int err_no = 0;
            string err_msg = string.Empty;
            try
            {
               
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@ClientId", ClientId),
                    new SqlParameter("@StoreName", dto.StoreName),
                    new SqlParameter("@StoreType", dto.StoreType),
                    new SqlParameter("@StoreEmail", dto.StoreEmail ?? string.Empty),
                    new SqlParameter("@PhoneNumber", dto.PhoneNumber ?? string.Empty),
                    new SqlParameter("@AlternatePhone", dto.AlternatePhoneNumber ?? string.Empty),
                    new SqlParameter("@DrugLicenseNumber", dto.DrugLicenseNumber ?? string.Empty),
                    new SqlParameter("@AddressLine1", dto.AddressLine1 ?? string.Empty),
                    new SqlParameter("@AddressLine2", dto.AddressLine2 ?? string.Empty),
                    new SqlParameter("@CountryId", dto.CountryId),
                    new SqlParameter("@StateId", dto.StateId ),
                    new SqlParameter("@CityId", dto.CityId),
                    new SqlParameter("@PostalCode", dto.PostalCode ?? string.Empty),
                    new SqlParameter("@Latitude", dto.Latitude ?? null),
                    new SqlParameter("@Longitude", dto.Longitude ?? null),
                    new SqlParameter("@IsOnBording", dto.IsOnBording),
                    new SqlParameter("@UserId", dto.UserId <= 0 ? UserId : dto.UserId),
                    new SqlParameter("@CreatedBy", UserId),
                    new SqlParameter("@ErrNumber", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    },
                    new SqlParameter("@ErrMsg", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    }
                };
                var result = ExecuteStoredProcedure(StoredProcedure.Sp_AddStore, param, _unitOfWork.GetConnection());
                err_no = param.First(p => p.ParameterName == "@ErrNumber").Value != DBNull.Value
                    ? Convert.ToInt32(param.First(p => p.ParameterName == "@ErrNumber").Value) : 0;
                err_msg = param.First(p => p.ParameterName == "@ErrMsg").Value?.ToString() ?? "";

                if (err_no != 0)
                    return ApiResponse<StoreResponseDto>.Fail(err_no, err_msg);

                var dbResult = CommonMethod.ConvertToList<StoreResponseDto>(result.Tables[0]).FirstOrDefault();
                return ApiResponse<StoreResponseDto>.Success(dbResult, err_msg);
                //return ApiResponse<LoginResponseDto>.Success(null, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<StoreResponseDto>.Fail(500, $"An error occurred during Update Client: {ex.Message}");
            }
        }
    }
}
