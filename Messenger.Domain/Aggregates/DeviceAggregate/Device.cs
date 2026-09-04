using Messenger.Domain.Abstractions;
using Messenger.Domain.Aggregates.UserAggregate;

namespace Messenger.Domain.Aggregates.DeviceAggregate;

public class Device : AggregateRoot
{
    public Guid UserId { get; private set; }

    public string DeviceName { get; private set; }      // "Pixel 6", "iPhone 14"
    public string DeviceType { get; private set; }      // "Android", "iOS", "Web"
    public string DeviceId { get; private set; }        // Уникальный ID устройства

    // E2EE Keys для этого устройства
    public PublicKey IdentityKey { get; private set; }
    public List<PublicKey> OneTimePreKeys { get; private set; }
    public long SignedPreKeyId { get; private set; }

    // Session
    public string RefreshToken { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public bool IsActive { get; private set; }
}
