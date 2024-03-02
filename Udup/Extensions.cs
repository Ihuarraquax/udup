﻿using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Udup;

public static class Extensions
{
    public static IServiceCollection AddUdup(this IServiceCollection services)
    {
        services.AddTransient<Parser>();
        return services;
    }    
    
    public static IApplicationBuilder UseUdup(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var httpMethod = context.Request.Method;
            var path = context.Request.Path.Value;
            
            if (httpMethod == "GET" && path.EndsWith("udup"))
            {
                var result = context.RequestServices.GetRequiredService<Parser>().Parse(AppDomain.CurrentDomain.GetAssemblies());
                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
                return;
            }
            
            await next(context);
        });
        
        return app;
    }
}