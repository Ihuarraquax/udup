using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Udup.AspNet;
using Udup.WebApp;
using Udup.WebApp.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<Udup.WebApp.Program>();
} );
builder.Services.AddUdup();
builder.Services.AddScoped<IDomainEventBService, DomainEventBService>();
builder.Services.AddDbContext<DatabaseContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseUdup();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapDomainEventBEndpointsWithService();

app.MapGet("/domainEventA", ([FromServices] IMediator mediator) =>
    {
        mediator.Publish(new DomainEventAHappened());
    })
    .WithOpenApi();

app.MapGet("/domainEventB", ([FromServices] IMediator mediator, IDomainEventBService service) => service.SendBEvent())
    .WithOpenApi();

app.MapGet("/domainEventC", async ([FromServices] IMediator mediator, DatabaseContext db) =>
    {
        var sample = new Sample();
        sample.AddDomainEvent(new DomainEventCHappened());
        db.Samples.Add(sample);
        await db.SaveEntitiesAsync();
    })
    .WithOpenApi();

app.MapGet("/domainEventD", async ([FromServices] IMediator mediator, DatabaseContext db) =>
    {
        var sample = new Sample();
        sample.MakeActionD();
        db.Samples.Add(sample);
        await db.SaveEntitiesAsync();
    })
    .WithOpenApi();

app.Run();


namespace Udup.WebApp
{
    public partial class Program { }
}