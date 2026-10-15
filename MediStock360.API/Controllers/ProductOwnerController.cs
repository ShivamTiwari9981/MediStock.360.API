using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductOwnerController : ControllerBase
    {
        private readonly IProductOwnerService _productOwnerService;

        public ProductOwnerController(IProductOwnerService productOwnerService)
        {
            _productOwnerService = productOwnerService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _productOwnerService.GetDashboardStatsAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients([FromQuery] string? searchTerm, [FromQuery] bool? isActive)
        {
            var result = await _productOwnerService.GetAllClientsAsync(searchTerm, isActive);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("clients/{clientId:long}")]
        public async Task<IActionResult> GetClientDetails(long clientId)
        {
            var result = await _productOwnerService.GetClientDetailsAsync(clientId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Only Product Owner can create new clients.
        /// </summary>
        [HttpPost("clients")]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientByPoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productOwnerService.CreateClientAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("clients/{clientId:long}/status")]
        public async Task<IActionResult> UpdateClientStatus(long clientId, [FromQuery] bool isActive)
        {
            var result = await _productOwnerService.UpdateClientStatusAsync(clientId, isActive);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Only Product Owner can read and provide client configurations and settings.
        /// </summary>
        [HttpGet("clients/{clientId:long}/configurations")]
        public async Task<IActionResult> GetClientConfigurations(long clientId)
        {
            var result = await _productOwnerService.GetClientConfigurationAsync(clientId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Only Product Owner can give / save client configurations.
        /// </summary>
        [HttpPost("clients/configurations")]
        public async Task<IActionResult> SaveClientConfigurations([FromBody] ClientConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productOwnerService.SaveClientConfigurationAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("stores")]
        public async Task<IActionResult> GetAllStores([FromQuery] long? clientId, [FromQuery] string? searchTerm)
        {
            var result = await _productOwnerService.GetAllStoresAsync(clientId, searchTerm);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("approvals")]
        public async Task<IActionResult> GetApprovals([FromQuery] byte? status)
        {
            var result = await _productOwnerService.GetApprovalsAsync(status);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("approvals/action")]
        public async Task<IActionResult> ProcessApprovalAction([FromBody] ApprovePaymentActionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productOwnerService.ProcessApprovalActionAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

