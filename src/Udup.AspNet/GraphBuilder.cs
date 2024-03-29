﻿using System.Text;
using Udup.Abstractions;

namespace Udup.AspNet;

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

        // foreach (var trace in result.Traces)
        // {
        //     graphBuilder.AppendLine($"{trace.Trace.Id}[\"{trace.Trace.Name}\"]");
        //     graphBuilder.AppendLine($"{trace.Trace.Id} --> {trace.Event.Id}");
        // }

        return graphBuilder.ToString();
    }
}