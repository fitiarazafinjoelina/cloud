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
    public async Task<IActionResult> SendEmail(String from, String to, String subject, String body)
    {
        try
        {
            await _emailService.SendEmailAsync(from,to,subject,body);
            return Ok(new { message = "Email sent successfully!" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
        }
    }

    [HttpPost("send-with-image")]
    public async Task<IActionResult> SendEmailWithImage(String from, String to, String subject, String body,String imagePath,String attachmentPath)
    {
        try
        {
            await _emailService.SendEmailWithImageAsync(from, to, subject, body, imagePath, attachmentPath);
            return Ok(new { message = "Email with image sent successfully!" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email with image.", error = ex.Message });
        }
    }

    [HttpPost("send-from-template")]
    public async Task<IActionResult> SendEmailFromTemplate(String from, String to, String subject,String templatePath,String name,String message,String imagePath)
    {
        try
        {
            await _emailService.SendEmailFromTemplateAsync(from, to, subject, templatePath, name, message, imagePath);
            return Ok(new { message = "Email sent from template successfully!" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while sending the email from template.", error = ex.Message });
        }
    }
}
