using Messenger.Domain.Aggregates.UserAggregate;
using Messenger.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Api.Context.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("tUsers");
        builder.HasKey(u => u.Id);

        // Value Object: UserName
        builder.Property(u => u.Username)
            .HasConversion(name => name.Value, value => UserName.Create(value))
            .HasMaxLength(20);

        // Value Object: Email
        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(320);

        builder.Property(u => u.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.Status).HasConversion<int>();
        builder.Property(u => u.PasswordHash).IsRequired();

        // Uri -> string (null не проходит через конвертер)
        builder.Property(u => u.AvatarUrl)
            .HasConversion(url => url!.ToString(), value => new Uri(value));

        // Value Object: UserProfile (complex type)
        builder.ComplexProperty(u => u.Profile, profile =>
        {
            profile.Property(p => p.DisplayName).HasMaxLength(100);
            profile.Property(p => p.Bio).HasMaxLength(500);
        });

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}