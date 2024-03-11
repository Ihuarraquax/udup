using System.Reflection;

namespace Udup.UdupReflection;

public static class Parser
{
    public static UdupResponse Parse(Assembly[] assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsAssignableTo(typeof(IUdupHandler)))
            .Where(p => p.IsClass);

        var allEventTypes = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsAssignableTo(typeof(IUdupMessage)))
            .Where(p => p.IsClass);

        return new UdupResponse(allEventTypes.Select(_ => _.Name).ToArray()
            ,
            EventHandlers(handlerTypes));
    }

    private static List<EventHandler> EventHandlers(IEnumerable<Type> handlerTypes)
    {
        var result =  handlerTypes.Select(handler =>
            new EventHandler(handler.Name,
                handler.GetInterfaces()
                    .Where(@interface =>
                        @interface.GetInterfaces()
                            .FirstOrDefault() == typeof(IUdupHandler))
                    .Select(_ => _.GenericTypeArguments.First().Name)
                    .ToArray()
            )).ToList();
        return result;
    }
}