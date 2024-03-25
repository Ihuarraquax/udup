using Udup.Abstractions;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core;

public interface IGatherEventHandlers
{
    List<EventHandler> GatherEventHandlers();
}

public interface IGatherEvents
{
    List<IdAndName> GatherEvents();
}

public interface IGatherEventTraces
{
    List<EventTrace> GatherEventTraces();
}