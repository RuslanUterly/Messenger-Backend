namespace Messenger.Api.Infrastructure.Email;

public class SmtpOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string FromEmail { get; set; }
    public required string FromName { get; set; } = "CheckIN";
}
