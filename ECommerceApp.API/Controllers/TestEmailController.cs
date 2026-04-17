using ECommerceApp.API.Models;
using ECommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("api/test-email")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendAsync([FromBody] TestEmailRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail))
                return BadRequest(new { message = "ToEmail is required." });

            var htmlBody =
                $"""
                <h2>Test Mail</h2>
                <p>Bu e-posta deneme amaciyla gonderildi.</p>
                <p>{request.Message ?? "Mail servisi basariyla calisiyor."}</p>
                <p>Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                """;

            await _emailService.SendAsync(request.ToEmail, request.Subject, htmlBody, cancellationToken);

            return Ok(new
            {
                message = "Test email sent successfully."
            });
        }
    }
}
