using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IWebHostEnvironment _env;

        public PaymentController(IPaymentService paymentService, IWebHostEnvironment env)
        {
            _paymentService = paymentService;
            _env = env;
        }

        [HttpGet("payment-method")]
        public async Task<IActionResult> GetPaymentMothods()
        {
            var result = await _paymentService.GetPaymentMethod();
            if (!result.IsSuccess)
                return BadRequest(result);

            // Ensure QR code URLs are absolute so UI can load the images
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}".TrimEnd('/');
                var wwwroot = _env.WebRootPath ?? Path.Combine(System.AppContext.BaseDirectory, "wwwroot");

                if (result.Data?.PaymentConfigurations != null)
                {
                    foreach (var cfg in result.Data.PaymentConfigurations)
                    {
                        var qr = cfg.QRCodeUrl?.Trim();

                        // If already an absolute URL, leave it
                        if (!string.IsNullOrEmpty(qr) && (qr.StartsWith("http://") || qr.StartsWith("https://")))
                            continue;

                        string candidatePath = null;

                        if (!string.IsNullOrEmpty(qr))
                        {
                            // treat qr as relative path or filename
                            var relative = qr.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                            candidatePath = Path.Combine(wwwroot, relative);
                            if (System.IO.File.Exists(candidatePath))
                            {
                                cfg.QRCodeUrl = baseUrl + "/" + relative.Replace(Path.DirectorySeparatorChar, '/');
                                continue;
                            }
                        }

                        // Fallback: look for a default QR file under wwwroot/payment
                        var defaultRelative = "payment/upi-qr.jpeg";
                        var defaultPath = Path.Combine(wwwroot, defaultRelative.Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(defaultPath))
                        {
                            cfg.QRCodeUrl = baseUrl + "/" + defaultRelative;
                        }
                        else
                        {
                            // leave as null if no file exists
                            cfg.QRCodeUrl = null;
                        }
                    }
                }
            }
            catch
            {
                // ignore failures mapping URLs to avoid breaking API; client will handle missing images
            }

            return Ok(result);
        }

        [HttpPost("submit-client-payment")]
        [HttpPost("offline-payment")]
        public async Task<IActionResult> SubmitClientPayment([FromBody] SaveClientPaymentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.SubmitClientPaymentAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("approve-payment")]
        public async Task<IActionResult> ApprovePayment([FromBody] ApprovePaymentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.ApproveClientPaymentAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("test-telegram")]
        public async Task<IActionResult> TestTelegram([FromQuery] string? message)
        {
            var result = await _paymentService.TestTelegramBotAsync(message ?? "MediStock360 Telegram Bot is connected and active!");
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
