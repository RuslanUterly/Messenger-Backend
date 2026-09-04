using System.Net.Mail;
using Messenger.Domain.Abstractions;

namespace Messenger.Domain.ValueObjects;

public record Email
{
    public string Value { get; init; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email не может быть пустым");

        string normalizedEmail = email.Trim().ToLowerInvariant();

        try
        {
            var mailAddress = new MailAddress(normalizedEmail);
            return new Email(mailAddress.Address);
        }
        catch (FormatException)
        {
            throw new DomainException($"'{email}' не прошел валидацию");
        }
    }

    // Optional: Implicit conversion allows you to pass this object directly where a string is expected
    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}

