using System.Collections.Generic;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;

namespace MediStock360.Application.Interfaces
{
    public interface IMasterService
    {
        Task<ApiResponse<List<CountryResponseDto>>> GetAllCountriesAsync(GetCountriesRequestDto request = null);
        Task<ApiResponse<List<StateResponseDto>>> GetStatesByCountryIdAsync(GetStatesByCountryRequestDto request);
        Task<ApiResponse<List<StateResponseDto>>> GetStatesByCountryIdAsync(int countryId, bool? isActive = null);
        Task<ApiResponse<List<CityResponseDto>>> GetCitiesByStateIdAsync(GetCitiesByStateRequestDto request);
        Task<ApiResponse<List<CityResponseDto>>> GetCitiesByStateIdAsync(int stateId, bool? isActive = null);
    }
}

