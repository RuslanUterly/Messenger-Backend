using Messenger.Domain.Aggregates.ConversationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Api.Context.Configurations;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("tConversations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Type).HasConversion<string>();
        builder.Property(c => c.Title).HasMaxLength(150);

        builder.Property(c => c.AvatarUrl)
            .HasConversion(url => url!.ToString(), value => new Uri(value));

        // Глобальный фильтр, иссключаем удаленные чаты
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasMany(c => c.Participants)
            .WithOne()
            .HasForeignKey(c => c.ConversationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}