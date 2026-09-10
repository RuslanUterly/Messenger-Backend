using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.DeviceAggregate;

public class Device : AggregateRoot
{
    public Guid UserId { get; private set; }

    public string DeviceName { get; private set; }      // "Pixel 6", "iPhone 14"
    public DeviceType DeviceType { get; private set; }      // "Android", "iOS", "Web"
    public string DeviceId { get; private set; }        // Уникальный ID устройства

    // E2EE Keys для этого устройства
    public PublicKey IdentityKey { get; private set; }
    public SignedPreKey SignedPreKey { get; private set; }
    private List<OneTimePreKey> _oneTimePreKeys = new();
    public IReadOnlyCollection<OneTimePreKey> OneTimePreKeys => _oneTimePreKeys.AsReadOnly();

    // Session
    public string RefreshTokenHash { get; private set; }
    public DateTimeOffset LastActivityAt { get; private set; }
    public bool IsActive { get; private set; }
}

public record SignedPreKey(long Id, PublicKey Key, string Signature);
public record OneTimePreKey(long Id, PublicKey Key);
public record PublicKey(string Algorithm, string KeyData); // X25519, Base64

public enum DeviceType 
{ 
    Android = 1, 
    IOS, 
    Web,
    Desktop 
}
