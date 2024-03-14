using System.Text;

namespace Udup.Core;

internal static class GraphBuilder
{
    public static string Build(UdupResponse result)
    {
        var graphBuilder = new StringBuilder("graph LR\n");

        foreach (var handler in result.Handlers)
        {
            graphBuilder.AppendLine($"{handler.Handler.Id}(\"{handler.Handler.Name}\")");

            foreach (var handlingEvent in handler.Events)
            {
                graphBuilder.AppendLine($"{handlingEvent.Id} --> {handler.Handler.Id}");
            }
        }

        foreach (var @event in result.Events)
        {
            graphBuilder.AppendLine($"{@event.Id}((\"{@event.Name}\"))");
        }

        foreach (var trace in result.Traces)
        {
            graphBuilder.AppendLine($"{trace.Name.Id}[\"{trace.Name.Name}\"]");
            graphBuilder.AppendLine($"{trace.Name.Id} --> {trace.Event.Id}");
        }

        return graphBuilder.ToString();
    }
}