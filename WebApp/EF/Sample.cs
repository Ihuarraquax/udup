using MediatR;

namespace Udup.WebApp.EF;

public class Sample
{
    public Guid Id { get; set; }
    
    private List<INotification> domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents ??= new List<INotification>();
        domainEvents.Add(eventItem);
    }
    
    public void ClearDomainEvents()
    {
        domainEvents?.Clear();
    }

    public void MakeActionD()
    {
        AddDomainEvent(new DomainEventDHappened());
    }
}