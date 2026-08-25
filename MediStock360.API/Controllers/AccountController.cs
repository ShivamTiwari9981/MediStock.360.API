using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOTPService _otpService;

        public AccountController(IAuthService authService, IOTPService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignupRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SignUpAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("switch-store")]
        public async Task<IActionResult> SwitchStore([FromBody] SwitchStoreRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SwitchStoreAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("switch-client")]
        public async Task<IActionResult> SwitchClient([FromBody] SwitchClientRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SwitchClientAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromQuery] string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email is required");
            }

            var result = await _otpService.SendOtpAsync(userEmail);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] OtpVerificationRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.email))
            {
                return BadRequest("User email is required");
            }
            if (string.IsNullOrWhiteSpace(dto.otp))
            {
                return BadRequest("OTP is required");
            }

            var result = await _otpService.VerifyEmailOTP(dto.email, dto.otp);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPassword(dto.UserEmail, dto.Password);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

