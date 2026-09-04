using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.MessageAggregate;

public class MessageStatus : Entity
{
    public Guid UserId { get; private set; }
    public MessageStatusType Status { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? ErrorReason { get; private set; }
    
    public void UpdateStatus(MessageStatusType newStatus)
    {
        Status = newStatus;
    }
}

public enum MessageStatusType
{
    Pending = 1,
    Sent,
    Delivered,
    Read,
    Failed,
    Deleted
}