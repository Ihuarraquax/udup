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