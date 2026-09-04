namespace Messenger.Domain.Aggregates.ConversationAggregate;

public class Conversation
{
    public ConversationType Type { get; private set; }

    // Metadata
    public string Title { get; private set; }
    public Uri? AvatarUrl { get; private set; }

    // Participants
    private List<Participant> _participants = new();
    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

    // Settings (jsonb)
    public ConversationSettings Settings { get; private set; }
    
    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastMessageAt { get; private set; }
    public string? LastMessagePreview { get; private set; } // Последнее сообщение (для списка)
    
    // State
    public bool IsArchived { get; private set; }
    public bool IsDeleted { get; private set; }
}

public enum ConversationType
{
    Direct = 1,
    Group,
    Broadcast,
}

public record ConversationSettings(
    bool IsEncrypted,                    // Всегда true для E2EE
    bool IsMuted,                        // Отключены уведомления
    bool IsPinned                        // Закреплён в списке
);

