using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Udup;
using Xunit;

namespace UdupTests.AspNet;

public class BasicTests 
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient client;

    public BasicTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUdupEndpoint()
    {
        // Arrange

        // Act
        var response = await client.GetFromJsonAsync<UdupResponse>("/udup.json");

        // Assert
        response.Events.Should().HaveCount(2);
        response.EventHandlers.Should().HaveCount(3);
    }
}