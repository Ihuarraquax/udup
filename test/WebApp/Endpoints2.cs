using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Udup.Abstractions;

namespace Udup.WebApp;

public record DomainEventXHappened : IUdupMessage;

public static class Endpoints2
{
    public static void MapDomainEventBEndpointsWithService2(this WebApplication app)
    {
        app.MapGet("domainEventX",
            () => { new DomainEventXHappened(); });

        app.MapGet("domainEventXService",
                (IDomainEventXService service) => service.SendXEvent())
            .WithOpenApi();

        app.MapPost("domainEventXDomain",
                (IDomainEventBService service) =>
                {
                    var actioner = new XActioner();
                    actioner.MakeActionX();
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
    public async Task SendXEvent()
    {
        new DomainEventXHappened();
    }
}

public class XActioner
{
    public Guid Id { get; set; }
    
    private List<IUdupMessage> domainEvents;

    public IReadOnlyCollection<IUdupMessage> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(IUdupMessage eventItem)
    {
        domainEvents.Add(eventItem);
    }
    
    public void MakeActionX()
    {
        AddDomainEvent(new DomainEventXHappened());
    }
}