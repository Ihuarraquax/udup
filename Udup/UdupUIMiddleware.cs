﻿using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Udup;

public class UdupUiMiddleware
{
    private readonly RequestDelegate next;

    public UdupUiMiddleware(
        RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var httpMethod = context.Request.Method;
        var path = context.Request.Path.Value;
            
        if (httpMethod == "GET" && path.EndsWith("udup.json"))
        {
            var result = Parser.Parse(AppDomain.CurrentDomain.GetAssemblies());
            var json = JsonSerializer.Serialize(result);
            await context.Response.WriteAsync(json);
            return;
        }
        await next(context);
            
        if (httpMethod == "GET" && path.EndsWith("udup"))
        {
            await RespondWithIndexHtml(context.Response);
            return;
        }
    }

    private async Task RespondWithIndexHtml(HttpResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "text/html;charset=utf-8";

        using (var stream = typeof(UdupUiMiddleware).Assembly.GetManifestResourceStream($"{typeof(UdupUiMiddleware).Namespace}.index.html"))
        {
            using var reader = new StreamReader(stream);

            // Inject arguments before writing to response
            var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
            var result = Parser.Parse(AppDomain.CurrentDomain.GetAssemblies());
            var json = JsonSerializer.Serialize(result);
            htmlBuilder.Replace("__Events__", json);
            await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }
    }
}