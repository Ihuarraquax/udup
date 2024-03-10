using MediatR;
using Microsoft.AspNetCore.Mvc;
using Udup;
using Udup.WebApp;
using Udup.WebApp.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<Program>();
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


public partial class Program { }