using Messenger.Domain.Aggregates.DeviceAggregate;
using Messenger.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Api.Context.Configurations;

public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("tDevices");
        builder.HasKey(d => d.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(d => d.DeviceName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.DeviceId).HasMaxLength(100).IsRequired();
        builder.Property(d => d.DeviceType).HasConversion<string>();
        builder.Property(d => d.RefreshTokenHash).HasMaxLength(256).IsRequired();

        builder.HasIndex(d => new { d.UserId, d.DeviceId }).IsUnique();

        // Value Object: PublicKey (complex type)
        builder.ComplexProperty(d => d.IdentityKey, key =>
        {
            key.Property(k => k.Algorithm).HasMaxLength(32);
            key.Property(k => k.KeyData).HasMaxLength(512);
        });

        // SignedPreKey и OneTimePreKeys хранятся в jsonb-колонках.
        builder.OwnsOne(d => d.SignedPreKey, signedPreKey =>
        {
            signedPreKey.ToJson();
            signedPreKey.Property(k => k.Id).ValueGeneratedNever();
            signedPreKey.Property(k => k.Signature).HasMaxLength(512);

            signedPreKey.OwnsOne(k => k.Key, keyBuilder =>
            {
                keyBuilder.ToJson();
            });
        });

        builder.OwnsMany(d => d.OneTimePreKeys, preKeys =>
        {
            preKeys.ToJson();
            preKeys.Property(k => k.Id).ValueGeneratedNever();

            preKeys.OwnsOne(k => k.Key, keyBuilder =>
            {
                keyBuilder.ToJson();
            });
        });

        builder.Navigation(d => d.OneTimePreKeys).HasField("_oneTimePreKeys");
    }
}