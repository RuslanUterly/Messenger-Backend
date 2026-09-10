using Messenger.Domain.Abstractions;
using Messenger.Domain.ValueObjects;

namespace Messenger.Domain.Aggregates.UserAggregate;

public class User : AggregateRoot
{
    // Identity
    public UserName Username { get; private set; } 
    public Email Email { get; private set; }

     // Authentication
    public string PasswordHash { get; private set; } // bcrypt
    
    // Profile
    public UserProfile Profile { get; private set; }
    public Uri? AvatarUrl { get; private set; }
}

public record UserProfile(string DisplayName, string? Bio);