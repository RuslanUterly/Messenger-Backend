using Messenger.Domain.Abstractions;
using Messenger.Domain.ValueObjects;

namespace Messenger.Domain.Aggregates.UserAggregate;

public class User : AggregateRoot
{
    // Identity
    public UserName Username { get; private set; } 
    public Email Email { get; private set; }
    public string Phone { get; private set; }

    // Authorization
    public Role Role { get; private set; }
    public UserStatus Status { get; private set; }

     // Authentication
    public string PasswordHash { get; private set; } // bcrypt
    public string? PasswordHint { get; private set; }

    // Profile
    public UserProfile Profile { get; private set; }
    public Uri? AvatarUrl { get; private set; }
}

public record UserProfile(string DisplayName, string? Bio);

public enum Role
{
    User,
    Admin
}

public enum UserStatus
{
    Active, 
    Banned
}