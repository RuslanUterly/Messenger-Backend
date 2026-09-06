using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.MessageAggregate;

public class MessageDelivery : Entity
{
    public Guid MessageId { get; private set; }
    public Guid UserId { get; private set; }
    public DeliveryStatus Status { get; private set; }  // Sent / Delivered / Failed
    public DateTimeOffset UpdatedAt { get; private set; }

    public void MarkDelivered(DateTimeOffset now)
    {
        if (Status == DeliveryStatus.Failed)
            throw new DomainException("Terminal status");
            
        Status = DeliveryStatus.Delivered;
        UpdatedAt = now;   // ← как и в MarkRead, ты забыл обновить время
    }
}

public enum DeliveryStatus { Sent = 1, Delivered, Failed }