using Messenger.Domain.Aggregates.ConversationAggregate;
using Messenger.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Api.Context.Configurations;

public sealed class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("tParticipants");
        builder.HasKey(p => p.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Role).HasConversion<string>();

        // Value Object: ParticipantSettings (complex type)
        builder.ComplexProperty(p => p.Settings, settings =>
        {
            settings.Property(s => s.IsArchived);
            settings.Property(s => s.IsMuted);
            settings.Property(s => s.IsPinned);
        });

        // Ссылка на последнее прочитанное сообщение — без FK,
        // чтобы не создавать циклы каскадного удаления между messages и participants.
        builder.Property(p => p.LastReadMessageId);
    }
}