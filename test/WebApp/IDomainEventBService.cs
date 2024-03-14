using MediatR;

namespace Udup.WebApp;

public interface IDomainEventBService
{
    Task SendBEvent();
}

class DomainEventBService : IDomainEventBService
{
    private readonly IMediator mediator;

    public DomainEventBService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task SendBEvent()
    {
        return mediator.Send(new DomainEventBHappened());
    }
}