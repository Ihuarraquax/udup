using System.Collections.Generic;

namespace Udup.Abstractions;

public class EventTrace
{
    public EventTrace(IdAndName @event, EventUsages eventUsages)
    {
        this.Event = @event;
        this.Usages = eventUsages;
    }

    public IdAndName Event { get; set; }
    public EventUsages Usages { get; set; }
}


public class EventUsages(List<Usage> DirectUsages);

public class Usage( string Path, List<Usage> IndirectUsages);