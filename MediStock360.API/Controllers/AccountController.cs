using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOTPService _otpService;
        private readonly IUserService _userService;

        public AccountController(IAuthService authService, IOTPService otpService, IUserService userService)
        {
            _authService = authService;
            _otpService = otpService;
            _userService = userService;
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserEmail))
            {
                return BadRequest("User email is required");
            }
            if(dto.OtpPurpose<=0)
                return BadRequest("Invalid Otp purpose");

            var result = await _userService.IsUserExist(dto.UserEmail);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            else
            {
               await _otpService.SendOtpAsync(dto.UserEmail,dto.OtpPurpose);
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

            var result = await _otpService.VerfyOtpAsync(dto.email, dto.otp);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP([FromBody] OtpResendRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.email))
            {
                return BadRequest("User email is required");
            }
           

            var result = await _otpService.ResendOtp(dto.email, dto.OtpPurpose);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPassword(dto.UserEmail, dto.ConfirmPassword);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("platform/register")]
        public async Task<IActionResult> PlateformRegister([FromBody] PlateformRegister dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.PlateformUserAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

