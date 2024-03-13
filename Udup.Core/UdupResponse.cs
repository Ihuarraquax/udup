using Udup.Core.Roslyn.Internals;

namespace Udup.Core;

public record UdupResponse(List<IdAndName> Events, List<EventHandler> Handlers, List<EventTrace> Traces);

public record EventHandler(IdAndName Handler, IdAndName[] Events);

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