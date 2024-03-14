using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Udup.WebApp.EF;

public partial class DatabaseContext
{
    
    private readonly IMediator mediator;

    public DatabaseContext(
        DbContextOptions<DatabaseContext> options,
        IMediator mediator)
        : base(options)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var aggregateRoots = ChangeTracker
            .Entries<Sample>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        aggregateRoots
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var @event in domainEvents)
        {
            await mediator.Publish((object)@event, cancellationToken);
        }
        
        await SaveChangesAsync(cancellationToken);

        return true;
    }
}