using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.ConversationAggregate;

public class Conversation : AggregateRoot
{
    public ConversationType Type { get; private set; }

    // Metadata
    public string? Title { get; private set; }
    public Uri? AvatarUrl { get; private set; }

    // Participants
    private List<Participant> _participants = new();
    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

    public DateTimeOffset? LastMessageAt { get; private set; }
    public bool IsDeleted { get; private set; }
}

public enum ConversationType
{
    Direct = 1,
    Group,
    Broadcast,
}
