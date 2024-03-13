using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Udup.WebApp;

public static class Endpoints
{
    public static void MapDomainEventAEndpointsWithService(this WebApplication app)
    {
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

