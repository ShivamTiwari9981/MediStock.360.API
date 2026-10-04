using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {

        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPut("update-client")]
        public async Task<IActionResult> UpdateClient([FromBody] ClientRequestDto dto)
        {
            if (dto.ClientKey == Guid.Empty)
                return BadRequest(ModelState);

            if(dto.ClientId<=0)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //dto.IsUpdate = true;

            //var result = await _clientService.AddUpdateClient(dto);
            bool result = true;
            if (!result)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
