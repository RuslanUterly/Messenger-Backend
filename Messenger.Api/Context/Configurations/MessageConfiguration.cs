using Messenger.Domain.Aggregates.ConversationAggregate;
using Messenger.Domain.Aggregates.MessageAggregate;
using Messenger.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Api.Context.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("tMessages");
        builder.HasKey(m => m.Id);

        builder.HasOne<Conversation>()
            .WithMany()
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Самоссылка для ответов (ReplyToId)
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(m => m.ReplyToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(m => m.Sequence).ValueGeneratedNever();
        builder.HasIndex(m => new { m.ConversationId, m.Sequence }).IsUnique();

        builder.Property(m => m.EncryptedContent).IsRequired();
        builder.Property(m => m.Type).HasConversion<string>();

        // Глобальный фильтр, иссключаем удаленные сообщения
        builder.HasQueryFilter(m => !m.IsDeleted);

        // Вложения — jsonb-колонка
        builder.OwnsMany(m => m.Attachments, attachments =>
        {
            attachments.ToJson();
            attachments.Property(a => a.FileName).HasMaxLength(255);
            attachments.Property(a => a.MimeType).HasMaxLength(100);
            attachments.Property(a => a.EncryptedUrl).HasMaxLength(1024);
            attachments.Property(a => a.ThumbnailUrl).HasMaxLength(1024);
        });
    }
}