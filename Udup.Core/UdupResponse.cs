namespace Udup.Core;

public record UdupResponse(List<EventWithTrace> Events, List<EventHandler> EventHandlers);

public record EventHandler(IdAndName Handler, IdAndName[] Events);

public class EventWithTrace
{
    public EventWithTrace(IdAndName @event, List<IdAndName> sources)
    {
        Event = @event;
        Sources = sources;
    }

    public IdAndName Event { get; set; }
    public List<IdAndName> Sources { get; set; }
}

public class IdAndName
{
    public IdAndName()
    {
        
    }
    
    public IdAndName(string id)
    {
        Id = id;
        Name = id;
    }
    
    public IdAndName(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}