namespace Udup;

public record UdupResponse(string[] Events, List<EventHandler> EventHandlers);

public record EventHandler(string Name, string Event);