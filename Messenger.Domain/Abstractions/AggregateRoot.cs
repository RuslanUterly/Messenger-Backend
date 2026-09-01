namespace Messenger.Domain.Abstractions;

public class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    // Вызывается инфраструктурным слоем (например, в EF Core SaveChanges) после сохранения в БД
    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}
