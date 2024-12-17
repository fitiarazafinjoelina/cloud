using cloud.helper;

namespace cloud.email;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
        }
    }
    [HttpPost("sendOTP")]
    public async Task<IActionResult> SendEmailOtp([FromBody] EmailDTO emailDto)
    {
        try
        {
            var otp = OTPHelper.GenerateOTP();
            await _emailService.SendEmailOTPAsync(emailDto.From, emailDto.To, otp);
            return Ok(new { message = "DTO email sent successfully!" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
        }
    }

    [HttpPost("send-with-image")]
    public async Task<IActionResult> SendEmailWithImage([FromBody] EmailDTO emailDto)
    {
        try
        {
            await _emailService.SendEmailWithImageAsync(emailDto.From,emailDto.To,emailDto.Subject,emailDto.Body,emailDto.ImagePath,emailDto.AttachmentPath);
            return Ok(new { message = "Email with image sent successfully!" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email with image.", error = ex.Message });
        }
    }

    [HttpPost("send-from-template")]
    public async Task<IActionResult> SendEmailFromTemplate([FromBody] EmailDTO emailDto)
    {
        try
        {
            await _emailService.SendEmailFromTemplateAsync(emailDto.From,emailDto.To,emailDto.Subject,emailDto.TemplatePath,emailDto.Name,emailDto.Message,emailDto.ImagePath);
            return Ok(new { message = "Email sent from template successfully!" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email from template.", error = ex.Message });
        }
    }
}
