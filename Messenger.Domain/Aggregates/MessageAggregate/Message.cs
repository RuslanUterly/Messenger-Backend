using Messenger.Domain.Abstractions;

namespace Messenger.Domain.Aggregates.MessageAggregate;

public class Message : AggregateRoot
{
    // Context
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }

    public long Sequence { get; private set; }

    // Content (E2EE encrypted)
    public string EncryptedContent { get; private set; }      // Зашифрованный JSON
    public MessageType Type { get; private set; }           // "text", "image", "file"
    
    // Threading
    public Guid? ReplyToId { get; private set; }         // Ответ на сообщение
    public Guid? EditOfId { get; private set; }          // Редактирование
    public bool IsDeleted { get; private set; }          // Soft delete
    
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

public enum MessageType { Text = 1, Image, Video, Voice, File }