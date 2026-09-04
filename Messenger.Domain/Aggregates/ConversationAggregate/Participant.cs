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

public record ConversationSettings(
    bool IsEncrypted,                    // Всегда true для E2EE
    bool IsMuted,                        // Отключены уведомления
    bool DisappearingMessages,           // Сообщения исчезают
    int? DisappearingMessagesTimeout,    // Через сколько секунд
    bool IsPinned                        // Закреплён в списке
);

public enum ParticipantRole 
{ 
    Admin = 1, 
    Member,
    ReadOnly 
}
