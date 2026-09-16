using Messenger.Domain.Aggregates.ConversationAggregate;
using Messenger.Domain.Aggregates.DeviceAggregate;
using Messenger.Domain.Aggregates.MessageAggregate;
using Messenger.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.Context;

public class MessengerContext(DbContextOptions<MessengerContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessengerContext).Assembly);
    }
}
