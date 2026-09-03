using Messenger.Domain.Abstractions;
using Messenger.Domain.ValueObjects;

namespace Messenger.Domain.Aggregates.UserAggregates;

public class User : AggregateRoot
{
    // Identity
    public UserName Username { get; private set; } 
    public string Email { get; private set; }

     // Authentication
    public string PasswordHash { get; private set; } // bcrypt
    
    // Profile
    public UserProfile Profile { get; private set; }
    public string Bio { get; private set; }
    public string AvatarUrl { get; private set; }

    // E2EE Keys (Signal Protocol)
    public PublicKey IdentityKey { get; private set; }      // Долгосрочный ключ
    public PublicKey SignedPreKey { get; private set; }     // Подписанный пре-ключ
    public List<PublicKey> OneTimePreKeys { get; private set; } // Одноразовые ключи
    
    // Security
    public SafetyNumber SafetyNumber { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    
    // Status
    public UserStatus Status { get; private set; } // Online/Offline/Away
    public bool IsActive { get; private set; }
}

public record UserProfile(string DisplayName, string? Bio);
public record PublicKey(string Algorithm, string KeyData); // X25519, Base64
public record SafetyNumber(string Value); // 60 цифр для верификации
public enum UserStatus { Online, Offline, Away }