using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.MessageAggregate;

public class Message : AggregateRoot
{
    // Context
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }

    // Content (E2EE encrypted)
    public string EncryptedContent { get; private set; }      // Зашифрованный JSON
    public string ContentType { get; private set; }           // "text", "image", "file"
    
    // Metadata
    public long Timestamp { get; private set; }               // Unix timestamp (клиентский)
    public long? ServerTimestamp { get; private set; }        // Unix timestamp (серверный)
    
    // E2EE
    public string MessageIdForProtocol { get; private set; }  // Message ID для Signal Protocol
    public string SenderChainKeyIndex { get; private set; }   // Для Ratchet
    public string SenderSignature { get; private set; }       // Подпись для верификации
    
    // Threading
    public Guid? ReplyToId { get; private set; }         // Ответ на сообщение
    public Guid? EditOfId { get; private set; }          // Редактирование
    public bool IsDeleted { get; private set; }               // Soft delete
    
    // Statuses
    private List<MessageStatus> _statuses = new();
    public IReadOnlyCollection<MessageStatus> Statuses => _statuses.AsReadOnly();
    
    // Attachments
    private List<MessageAttachment> _attachments = new();
    public IReadOnlyCollection<MessageAttachment> Attachments => _attachments.AsReadOnly();
}

public record MessageAttachment(
    string Id,
    string FileName,
    long FileSize,
    string MimeType,
    string EncryptedUrl,      // Зашифрованная ссылка на файл
    string ThumbnailUrl       // Для изображений
);
