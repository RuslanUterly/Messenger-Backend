using Messenger.Domain.Abstractions;
using Messenger.Domain.ValueObjects;

namespace Messenger.Domain.Aggregates.UserAggregates;

public class User : AggregateRoot
{
    public UserName Username { get; private set; } 
}