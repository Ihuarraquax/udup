using System.Text;

namespace Udup.Core;

internal static class GraphBuilder
{
    public static string Build(UdupResponse result)
    {
        var graphBuilder = new StringBuilder("graph LR\n");

        foreach (var handler in result.EventHandlers)
        {
            graphBuilder.AppendLine($"{handler.Name}({handler.Name})");

            foreach (var handlingEvent in handler.Events)
            {
                graphBuilder.AppendLine($"{handlingEvent} --> {handler.Name}");
            }
        }
        
        foreach (var @event in result.Events)
        {
            graphBuilder.AppendLine($"{@event.Name}(({@event.Name}))");
        }

        return graphBuilder.ToString();
    }
}