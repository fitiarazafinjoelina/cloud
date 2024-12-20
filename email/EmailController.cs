using Microsoft.AspNetCore.Mvc;

namespace cloud.email;


[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailDTO emailDto)
    {
        try
        {
            await _emailService.SendEmailAsync(emailDto.From,emailDto.To,emailDto.Subject,emailDto.Body);
            return Ok(new { message = "Email sent successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
        }
    }
}