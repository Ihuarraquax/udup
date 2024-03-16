using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Udup.Abstractions;

namespace Udup.WebApp;

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

public class DomainEventAHappenedHandlerD : INotificationHandler<DomainEventAHappened>, IUdupHandler<DomainEventAHappened>
{
    public Task Handle(DomainEventAHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class DomainEventBHappenedHandlerA : INotificationHandler<DomainEventBHappened>, IUdupHandler<DomainEventBHappened>
{
    public Task Handle(DomainEventBHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class DomainEventBAndCHappenedHandlerA : INotificationHandler<DomainEventBHappened>, INotificationHandler<DomainEventCHappened>, IUdupHandler<DomainEventBHappened>, IUdupHandler<DomainEventCHappened>
{
    public Task Handle(DomainEventBHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventCHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}