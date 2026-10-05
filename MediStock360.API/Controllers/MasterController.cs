using System.Collections.Generic;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        #region Countries

        /// <summary>
        /// Retrieves all countries with optional active status filter.
        /// </summary>
        [HttpGet("countries")]
        public async Task<IActionResult> GetAllCountries([FromQuery] GetCountriesRequestDto request)
        {
            var result = await _masterService.GetAllCountriesAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region States

        /// <summary>
        /// Retrieves states based on country ID via query parameter.
        /// </summary>
        [HttpGet("states")]
        public async Task<IActionResult> GetStatesByCountry([FromQuery] GetStatesByCountryRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _masterService.GetStatesByCountryIdAsync(request);
            if (!result.IsSuccess)
            {
                if (result.ErrorNo == 404)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves states based on country ID via route parameter.
        /// </summary>
        [HttpGet("states/{countryId:int}")]
        public async Task<IActionResult> GetStatesByCountryId(int countryId, [FromQuery] bool? isActive = null)
        {
            if (countryId <= 0)
                return BadRequest(ApiResponse<List<StateResponseDto>>.Fail(400, "CountryId must be greater than 0."));

            var result = await _masterService.GetStatesByCountryIdAsync(countryId, isActive);
            if (!result.IsSuccess)
            {
                if (result.ErrorNo == 404)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Cities

        /// <summary>
        /// Retrieves all cities based on state ID via query parameter.
        /// </summary>
        [HttpGet("cities")]
        public async Task<IActionResult> GetCitiesByState([FromQuery] GetCitiesByStateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _masterService.GetCitiesByStateIdAsync(request);
            if (!result.IsSuccess)
            {
                if (result.ErrorNo == 404)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all cities based on state ID via route parameter.
        /// </summary>
        [HttpGet("cities/{stateId:int}")]
        public async Task<IActionResult> GetCitiesByStateId(int stateId, [FromQuery] bool? isActive = null)
        {
            if (stateId <= 0)
                return BadRequest(ApiResponse<List<CityResponseDto>>.Fail(400, "StateId must be greater than 0."));

            var result = await _masterService.GetCitiesByStateIdAsync(stateId, isActive);
            if (!result.IsSuccess)
            {
                if (result.ErrorNo == 404)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion
    }
}

