
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using static MediStock360.Application.Common.GenericProcedureCall;

namespace MediStock360.Application.Services
{
    public class ClientService : BaseService, IClientService
    {
        public ClientService(IUnitOfWork unitOfWork, ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {

        }
        public async Task<bool> IsClientExist()
        {
            var result = await _unitOfWork.ClientRepository.AnyAsync(x => x.ClientId == ClientId && x.ClientKey == ClientKey);
            return result;
        }
        public async Task<ApiResponse<ClientResponseDto>> UpdateClient(ClientRequestDto dto)
        {
            int err_no = 0;
            string err_msg = string.Empty;
            try
            {
                // 1. Find user by Email or UserName
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(
                    u => u.ClientId == ClientId);

                if (client == null)
                {
                    return ApiResponse<ClientResponseDto>.Fail(1, "Invalid username/email or password.");
                }

                // 2. Verify password

                if (client.ClientKey != dto.ClientKey)
                {
                    return ApiResponse<ClientResponseDto>.Fail(1, "Client Key is not found");
                }
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@ClientId", ClientId),
                    new SqlParameter("@ClientName", dto.ClientName),
                    new SqlParameter("@CompanyName", dto.CompanyName),
                    new SqlParameter("@OwnerName", dto.OwnerName),
                    new SqlParameter("@BusinessTypeId", dto.BusinessTypeId),
                    new SqlParameter("@Email", dto.Email),
                    new SqlParameter("@Phone", dto.Phone),
                    new SqlParameter("@Gstnumber", dto.Gstnumber),
                    new SqlParameter("@DrugLicenseNumber", dto.DrugLicenseNumber),
                    new SqlParameter("@Address", dto.Address),
                    new SqlParameter("@CityId", dto.CityId),
                    new SqlParameter("@StateId", dto.StateId),
                    new SqlParameter("@CountryId", dto.CountryId),
                    new SqlParameter("@PostalCode", dto.PostalCode),
                    new SqlParameter("@UpdatedBy", UserId),
                    new SqlParameter("@ErrNumber", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    },
                    new SqlParameter("@ErrMsg", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    }
                };
                var result = ExecuteStoredProcedure(StoredProcedure.Sp_UpdateClient, param, _unitOfWork.GetConnection());
                err_no = param.First(p => p.ParameterName == "@ErrNumber").Value != DBNull.Value
                    ? Convert.ToInt32(param.First(p => p.ParameterName == "@ErrNumber").Value) : 0;
                err_msg = param.First(p => p.ParameterName == "@ErrMsg").Value?.ToString() ?? "";

                if (err_no != 0)
                    return ApiResponse<ClientResponseDto>.Fail(err_no, err_msg);

                var dbResult = CommonMethod.ConvertToList<ClientResponseDto>(result.Tables[0]).FirstOrDefault();
                return ApiResponse<ClientResponseDto>.Success(dbResult, err_msg);
                //return ApiResponse<LoginResponseDto>.Success(null, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClientResponseDto>.Fail(500, $"An error occurred during Update Client: {ex.Message}");
            }
        }
    }

}
