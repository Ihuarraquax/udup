using MediatR;
using Microsoft.AspNetCore.Mvc;
using Udup;
using Udup.WebApp;

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

app.Run();


public partial class Program { }