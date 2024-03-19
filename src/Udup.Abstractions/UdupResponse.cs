using System.Collections.Generic;

namespace Udup.Abstractions;

public class UdupResponse
{
    public List<IdAndName> Events { get; set; }
    public List<EventHandler> Handlers { get; set; }
    public List<EventTrace> Traces { get; set; }

    public UdupResponse()
    {
        
    }
    
    public UdupResponse(List<IdAndName> events, List<EventHandler> eventHandlers, List<EventTrace> eventTraces)
    {
        Events = events;
        Handlers = eventHandlers;
        Traces = eventTraces;
    }

};

public class EventHandler
{
    public IdAndName Handler { get; set; }
    public IdAndName[] Events { get; set; }

    public EventHandler()
    {
        
    }
    
    public EventHandler(IdAndName Handler, IdAndName[] Events)
    {
        this.Handler = Handler;
        this.Events = Events;
    }
}

public class EventTrace
{
    public EventTrace(IdAndName idAndName, IdAndName idAndName1)
    {
        Event = idAndName;
        Trace = idAndName1;
    }

    public IdAndName Event { get; set; }
    public IdAndName Trace { get; set; }
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