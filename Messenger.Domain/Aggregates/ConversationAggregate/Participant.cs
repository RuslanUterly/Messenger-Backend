using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.ConversationAggregate;

public class Participant : Entity
{
    public Guid UserId { get; private set; }
    public ParticipantRole Role { get; private set; } // Admin, Member, ReadOnly
    public DateTimeOffset JoinedAt { get; private set; }
    public DateTimeOffset? LastReadAt { get; private set; }
    public Guid? LastReadMessageId { get; private set; }

    // Settings (jsonb)
    public ParticipantSettings Settings { get; private set; }

    public void MarkRead(Guid messageId)
    {
        LastReadMessageId = messageId;
        LastReadAt = DateTimeOffset.UtcNow;
    }

    public void SetRole(ParticipantRole role)
    {
        Role = role;
    }
}

public record ParticipantSettings(
    bool IsArchived,                     // Чат вынесен в архив
    bool IsMuted,                        // Отключены уведомления
    bool IsPinned                        // Закреплён в списке
);

public enum ParticipantRole 
{ 
    Admin = 1, 
    Member,
    ReadOnly 
}
