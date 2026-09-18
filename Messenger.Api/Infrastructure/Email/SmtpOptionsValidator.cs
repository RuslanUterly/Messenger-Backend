using Microsoft.Extensions.Options;
using JasperFx.Core;

namespace Messenger.Api.Infrastructure.Email;

//public sealed class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
//{
//    public ValidateOptionsResult Validate(string? name, SmtpOptions options)
//    {
//        var failures = new List<string>();

//        if (options.Host.IsEmpty())
//            failures.Add("SmtpOptions:Host не задан в конфигурации.");

//        if (options.FromEmail.IsEmpty())
//            failures.Add("SmtpOptions:FromEmail не задан в конфигурации.");

//        return failures.Count > 0
//            ? ValidateOptionsResult.Fail(failures)
//            : ValidateOptionsResult.Success;
//    }
//}

