using Microsoft.Extensions.Options;

namespace Messenger.Api.Infrastructure.Email;

public sealed class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
{
    public ValidateOptionsResult Validate(string? name, SmtpOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Host))
            failures.Add("SmtpOptions:Host не задан в конфигурации.");

        if (string.IsNullOrWhiteSpace(options.FromEmail))
            failures.Add("SmtpOptions:FromEmail не задан в конфигурации.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

