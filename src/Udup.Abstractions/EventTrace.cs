namespace Udup.Abstractions;

public class EventTrace
{
    public EventTrace(IdAndName @event, IdAndName trace)
    {
        this.Event = @event;
        this.Trace = trace;
    }

    public IdAndName Event { get; set; }
    public IdAndName Trace { get; set; }
}