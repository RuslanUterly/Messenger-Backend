using JasperFx.Core;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Messenger.Api.Infrastructure.Email;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}

public partial class EmailSender(
    IOptions<SmtpOptions> options, 
    ILogger<EmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions options = options.Value;

    [LoggerMessage(Level = LogLevel.Information, Message = "Письмо отправлено на {ToEmail}")]
    private partial void LogEmailSent(string toEmail);

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(options.FromEmail, options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = options.EnableSsl,
        };

        if (options.UserName.IsNotEmpty())
        {
            client.Credentials = new NetworkCredential(options.UserName, options.Password);
        }

        await client.SendMailAsync(message);
        LogEmailSent(toEmail);
    }
}
