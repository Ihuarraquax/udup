using System.Reflection;

namespace Udup;

public static class Parser
{
    
    public static UdupResponse Parse(Assembly[] assemblies)
    {
        var type = typeof(IUdupHandler);
        var handlerTypes = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Where(p => p.IsClass);

        var allEventTypes = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsAssignableTo(typeof(IUdupMessage)))
            .Where(p => p.IsClass);

        return new UdupResponse(
            allEventTypes.Select(_ => _.Name).ToArray(),
            handlerTypes.Select(_ =>
                    new EventHandler(_.Name,
                        _.GetInterfaces()
                            .First(_ =>
                                _.GetInterfaces()
                                    .FirstOrDefault() == typeof(IUdupHandler))
                            .GenericTypeArguments
                            .First()
                            .Name)
                )
                .ToList());
    }
}