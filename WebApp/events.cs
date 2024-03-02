using MediatR;

namespace Udup.WebApp;


public record DomainEventAHappened : INotification, IUdupMessage;
public record DomainEventBHappened : INotification, IUdupMessage;

public class DomainEventAHappenedHandlerA : INotificationHandler<DomainEventAHappened>, IUdupHandler<DomainEventAHappened>
{
    public Task Handle(DomainEventAHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


public class DomainEventAHappenedHandlerB : INotificationHandler<DomainEventAHappened>, IUdupHandler<DomainEventAHappened>
{
    public Task Handle(DomainEventAHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class DomainEventAHappenedHandlerC : INotificationHandler<DomainEventAHappened>, IUdupHandler<DomainEventAHappened>
{
    public Task Handle(DomainEventAHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}