using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; init; }
    public string TokenHash { get; init; } = string.Empty;   // SHA-256 (hex)
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedById { get; init; }   // какой токен пришёл на замену

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;

    public void Revoke() => RevokedAt = DateTimeOffset.UtcNow;
}
