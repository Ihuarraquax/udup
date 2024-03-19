using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Udup.Abstractions;

namespace Udup.WebApp;

public record DomainEventAHappened : INotification, IUdupMessage;


public record DomainEventBHappened : INotification, IUdupMessage;


public record DomainEventCHappened : INotification, IUdupMessage;


public record DomainEventDHappened : INotification, IUdupMessage;

public static class Endpoints
{
    public static void MapDomainEventBEndpointsWithService(this WebApplication app)
    {
        app.MapGet("/domainEventA", ([FromServices] IMediator mediator) =>
        {
            var json = StaticUdupResponse.UdupJson;
            mediator.Publish(new DomainEventAHappened());
        });
        
        app.MapGet("/domainEventB",
                ([FromServices] IMediator mediator, IDomainEventBService service) => service.SendBEvent())
            .WithOpenApi();

        app.MapPost("/domainEventB",
                ([FromServices] IMediator mediator, IDomainEventBService service) => service.SendBEvent())
            .WithOpenApi();

        app.MapDelete("/domainEventB",
                ([FromServices] IMediator mediator, IDomainEventBService service) => service.SendBEvent())
            .WithOpenApi();

        app.MapPut("/domainEventB",
                ([FromServices] IMediator mediator, IDomainEventBService service) => service.SendBEvent())
            .WithOpenApi();

        app.MapPatch("/domainEventB",
                ([FromServices] IMediator mediator, IDomainEventBService service) => service.SendBEvent())
            .WithOpenApi();
    }
}