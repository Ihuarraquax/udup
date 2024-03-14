using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Udup.Abstractions;
using Udup.WebApp;

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
    public async Task GetUdupJsonEndpoint()
    {
        // Arrange

        // Act
        var response = await client.GetFromJsonAsync<UdupResponse>("/udup.json");

        // Assert
        await Verify(response);
    }
    
    [Fact]
    public async Task GetUdupDiagramEndpoint()
    {
        // Arrange

        // Act
        var html = await client.GetStringAsync("/udup");

        // Assert
        await Verify(html, "html");
    }
}