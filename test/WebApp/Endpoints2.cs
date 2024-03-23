using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Udup.Abstractions;
using Udup.WebApp.EF;

namespace Udup.WebApp;

public record DomainEventXHappened : INotification, IUdupMessage;

public static class Endpoints2
{
    public static void MapDomainEventBEndpointsWithService2(this WebApplication app)
    {
        app.MapGet("/domainEventX",
            ([FromServices] IMediator mediator) => { mediator.Publish(new DomainEventXHappened()); });

        app.MapGet("/domainEventXService",
                ([FromServices] IMediator mediator, IDomainEventXService service) => service.SendXEvent())
            .WithOpenApi();

        app.MapPost("/domainEventXDomain",
                ([FromServices] IMediator mediator, IDomainEventBService service) =>
                {
                    var sample = new Sample();
                    sample.MakeActionX();
                })
            .WithOpenApi();
    }
}

public interface IDomainEventXService
{
    Task SendXEvent();
}

class DomainEventXService : IDomainEventXService
{
    private readonly IMediator mediator;

    public DomainEventXService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task SendXEvent()
    {
        return mediator.Send(new DomainEventXHappened());
    }
}

public class XActioner
{
    public Guid Id { get; set; }
    
    private List<INotification> domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents.Add(eventItem);
    }
    
    public void MakeActionX()
    {
        AddDomainEvent(new DomainEventXHappened());
    }
}
