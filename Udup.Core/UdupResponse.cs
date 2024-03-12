namespace Udup.Core;

public record UdupResponse(List<EventWithTrace> Events, List<EventHandler> EventHandlers);

public record EventHandler(string Name, string[] Events);

public class EventWithTrace
{
    public EventWithTrace(string name, List<string> sources)
    {
        Name = name;
        Sources = sources;
    }

    public string Name { get; set; }
    public List<string> Sources { get; set; }
}