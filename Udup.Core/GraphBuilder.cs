using System.Text;

namespace Udup.Core;

internal static class GraphBuilder
{
    public static string Build(UdupResponse result)
    {
        var graphBuilder = new StringBuilder("graph LR\n");

        foreach (var handler in result.EventHandlers)
        {
            graphBuilder.AppendLine($"{handler.Handler.Id}(\"{handler.Handler.Name}\")");

            foreach (var handlingEvent in handler.Events)
            {
                graphBuilder.AppendLine($"{handlingEvent.Id} --> {handler.Handler.Id}");
            }
        }
        
        foreach (var @event in result.Events)
        {
            graphBuilder.AppendLine($"{@event.Event.Id}((\"{@event.Event.Name}\"))");
            
            foreach (var source in @event.Sources)
            {
                graphBuilder.AppendLine($"{source.Id}((\"{source.Name}\"))");
                graphBuilder.AppendLine($"{source.Id} --> {@event.Event.Name}");
            }
        }

        return graphBuilder.ToString();
    }
}