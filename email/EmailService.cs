namespace cloud.email;
using System;
using System.IO;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

public class EmailService
{
    private const string Host = "smtp.gmail.com";
    private const int Port = 587;
    private const string Email = "fitiarazafinjoelina10@gmail.com";
    private const string Password = "nyga twko oiko hags";
    
    public async Task SendEmailOtpAsync(String from, String to,String otp)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(from, Email));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = "Your one time pin";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"<p>Your one time pin is {otp}</p>"
        };

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(Email, Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        Console.WriteLine("Email sent successfully!");
    }

    public async Task SendEmailAsync(String from, String to, String subject, String body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(from, Email));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = "<p>"+body+"</p>"
        };

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(Email, Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        Console.WriteLine("Email sent successfully!");
    }

    public async Task SendEmailWithImageAsync(String from, String to, String subject, String body,String imagePath,String attachmentPath)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(from, Email));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = "<h1>"+body+"</h1><img src=\"cid:image1\">"
        };

        // Embed an image
        var image = bodyBuilder.LinkedResources.Add(imagePath);
        image.ContentId = "image1";

        // Add an attachment
        bodyBuilder.Attachments.Add(attachmentPath);

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(Email, Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        Console.WriteLine("Email with image sent successfully!");
    }

    public async Task SendEmailFromTemplateAsync(String from, String to, String subject,String templatePath,String name,String message,String imagePath)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(from, Email));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        // Read HTML template
        var templateContent = await File.ReadAllTextAsync(templatePath);

        // Replace placeholders in the template
        templateContent = templateContent.Replace("${name}", name);
        templateContent = templateContent.Replace("${message}", message);

        var bodyBuilder = new BodyBuilder { HtmlBody = templateContent };

        // Attach an inline image
        var image = bodyBuilder.LinkedResources.Add(imagePath);
        image.ContentId = "logoImage";

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(Email, Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        Console.WriteLine("Email from template sent successfully!");
    }
}
