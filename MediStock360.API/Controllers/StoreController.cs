using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost("add-store")]
        public IActionResult AddStore([FromBody] StoreRequestDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result =  _storeService.AddStore(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
