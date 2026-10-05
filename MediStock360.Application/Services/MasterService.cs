using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;

namespace MediStock360.Application.Services
{
    public class MasterService : BaseService, IMasterService
    {
        public MasterService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) : base(unitOfWork, currentUserService)
        {
        }

        #region Country Master

        /// <summary>
        /// Retrieves all countries with optional active status filter.
        /// </summary>
        public async Task<ApiResponse<List<CountryResponseDto>>> GetAllCountriesAsync(GetCountriesRequestDto request = null)
        {
            try
            {
                List<Country> countries;
                if (request != null && request.IsActive.HasValue)
                {
                    countries = await _unitOfWork.CountryRepository.WhereAsync(c => c.IsActive == request.IsActive.Value);
                }
                else
                {
                    countries = await _unitOfWork.CountryRepository.GetAllAsync();
                }

                var response = countries
                    .OrderBy(c => c.CountryName)
                    .Select(c => new CountryResponseDto
                    {
                        CountryId = c.CountryId,
                        CountryName = c.CountryName,
                        IsActive = c.IsActive
                    })
                    .ToList();

                return ApiResponse<List<CountryResponseDto>>.Success(response, "Countries retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CountryResponseDto>>.Fail(500, $"An error occurred while retrieving countries: {ex.Message}");
            }
        }

        #endregion

        #region State Master

        /// <summary>
        /// Retrieves states based on country ID.
        /// </summary>
        public async Task<ApiResponse<List<StateResponseDto>>> GetStatesByCountryIdAsync(int countryId, bool? isActive = null)
        {
            return await GetStatesByCountryIdAsync(new GetStatesByCountryRequestDto
            {
                CountryId = countryId,
                IsActive = isActive
            });
        }

        /// <summary>
        /// Retrieves states based on country ID using request model.
        /// </summary>
        public async Task<ApiResponse<List<StateResponseDto>>> GetStatesByCountryIdAsync(GetStatesByCountryRequestDto request)
        {
            try
            {
                if (request == null || request.CountryId <= 0)
                {
                    return ApiResponse<List<StateResponseDto>>.Fail(400, "A valid CountryId is required.");
                }

                var countryExists = await _unitOfWork.CountryRepository.AnyAsync(c => c.CountryId == request.CountryId);
                if (!countryExists)
                {
                    return ApiResponse<List<StateResponseDto>>.Fail(404, $"Country with ID {request.CountryId} was not found.");
                }

                List<State> states;
                if (request.IsActive.HasValue)
                {
                    states = await _unitOfWork.StateRepository.WhereAsync(s => s.CountryId == request.CountryId && s.IsActive == request.IsActive.Value);
                }
                else
                {
                    states = await _unitOfWork.StateRepository.WhereAsync(s => s.CountryId == request.CountryId);
                }

                var response = states
                    .OrderBy(s => s.StateName)
                    .Select(s => new StateResponseDto
                    {
                        StateId = s.StateId,
                        CountryId = s.CountryId,
                        StateName = s.StateName,
                        IsActive = s.IsActive
                    })
                    .ToList();

                return ApiResponse<List<StateResponseDto>>.Success(response, "States retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StateResponseDto>>.Fail(500, $"An error occurred while retrieving states: {ex.Message}");
            }
        }

        #endregion

        #region City Master

        /// <summary>
        /// Retrieves all cities based on state ID.
        /// </summary>
        public async Task<ApiResponse<List<CityResponseDto>>> GetCitiesByStateIdAsync(int stateId, bool? isActive = null)
        {
            return await GetCitiesByStateIdAsync(new GetCitiesByStateRequestDto
            {
                StateId = stateId,
                IsActive = isActive
            });
        }

        /// <summary>
        /// Retrieves all cities based on state ID using request model.
        /// </summary>
        public async Task<ApiResponse<List<CityResponseDto>>> GetCitiesByStateIdAsync(GetCitiesByStateRequestDto request)
        {
            try
            {
                if (request == null || request.StateId <= 0)
                {
                    return ApiResponse<List<CityResponseDto>>.Fail(400, "A valid StateId is required.");
                }

                var stateExists = await _unitOfWork.StateRepository.AnyAsync(s => s.StateId == request.StateId);
                if (!stateExists)
                {
                    return ApiResponse<List<CityResponseDto>>.Fail(404, $"State with ID {request.StateId} was not found.");
                }

                List<City> cities;
                if (request.IsActive.HasValue)
                {
                    cities = await _unitOfWork.CityRepository.WhereAsync(c => c.StateId == request.StateId && c.IsActive == request.IsActive.Value);
                }
                else
                {
                    cities = await _unitOfWork.CityRepository.WhereAsync(c => c.StateId == request.StateId);
                }

                var response = cities
                    .OrderBy(c => c.CityName)
                    .Select(c => new CityResponseDto
                    {
                        CityId = c.CityId,
                        StateId = c.StateId,
                        CountryId = c.CountryId,
                        CityName = c.CityName,
                        IsActive = c.IsActive
                    })
                    .ToList();

                return ApiResponse<List<CityResponseDto>>.Success(response, "Cities retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CityResponseDto>>.Fail(500, $"An error occurred while retrieving cities: {ex.Message}");
            }
        }

        #endregion
    }
}

