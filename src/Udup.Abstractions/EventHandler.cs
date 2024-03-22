namespace Udup.Abstractions;

public class EventHandler
{
    public IdAndName Handler { get; set; }
    public IdAndName[] Events { get; set; }

    public EventHandler()
    {
        
    }
    
    public EventHandler(IdAndName Handler, IdAndName[] Events)
    {
        this.Handler = Handler;
        this.Events = Events;
    }
}