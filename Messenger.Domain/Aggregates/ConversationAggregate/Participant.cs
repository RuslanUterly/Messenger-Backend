using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.ConversationAggregate;

public class Participant : Entity
{
    public Guid UserId { get; private set; }
    public ParticipantRole Role { get; private set; } // Admin, Member, ReadOnly
    public DateTime JoinedAt { get; private set; }
    public DateTime? LastReadAt { get; private set; }
    public Guid? LastReadMessageId { get; private set; }

    public void MarkRead(Guid messageId)
    {
        LastReadMessageId = messageId;
    }

    public void SetRole(ParticipantRole role)
    {
        Role = role;
    }
}

public enum ParticipantRole 
{ 
    Admin = 1, 
    Member,
    ReadOnly 
}
