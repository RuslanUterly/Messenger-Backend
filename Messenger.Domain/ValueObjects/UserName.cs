using Messenger.Domain.Abstractions;

namespace Messenger.Domain.ValueObjects;

public record UserName
{
    public string Value { get; }

    private UserName() {} //ef core

    private UserName(string value)
    {
        Value = value;
    }

    public static UserName Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException("Имя пользователя не может быть пустым.");

        var trimmed = input.Trim();
        if (trimmed.Length < 3 || trimmed.Length > 20)
            throw new DomainException("Имя должно быть от 3 до 20 символов.");

        foreach (var c in trimmed)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
                throw new DomainException("Разрешены только буквы, цифры и символ '_'.");
        }

        return new UserName(trimmed);
    }
}
