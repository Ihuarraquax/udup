using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Udup.Core;

public static class Extensions
{
    public static IServiceCollection AddUdup(this IServiceCollection services) => 
        services;

    public static IApplicationBuilder UseUdup(this IApplicationBuilder app) => 
        app.UseMiddleware<UdupUiMiddleware>();
}