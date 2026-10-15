using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;

        public SubscriptionPlanController(ISubscriptionPlanService subscriptionPlanService)
        {
            _subscriptionPlanService = subscriptionPlanService;
        }

        [HttpGet("plans")]
        public IActionResult GetSubscriptionPlan()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _subscriptionPlanService.GetSubscriptionPlan();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("confirm-plan")]
        [HttpPost("select-plan")]
        public async Task<IActionResult> ConfirmSubscriptionPlan([FromBody] ConfirmSubscriptionPlanRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _subscriptionPlanService.ConfirmSubscriptionPlanAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
