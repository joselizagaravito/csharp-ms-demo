using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HttpClient>(); // Configura HttpClient como un servicio compartido

var app = builder.Build(); // Construye la aplicación

// Define la ruta /call
app.MapGet("/call", async (HttpContext context, HttpClient client) =>
{
    try
    {
        var tracingHeaders = new[]
        {
            "x-request-id", "x-b3-traceid", "x-b3-spanid", "x-b3-sampled",
            "x-b3-parentspanid", "x-b3-flags", "x-ot-span-context"
        };

        var headersToSend = new Dictionary<string, string>();
        foreach (var header in tracingHeaders)
        {
            if (context.Request.Headers.TryGetValue(header, out var headerValue))
            {
                headersToSend[header] = headerValue.ToString();
            }
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "http://127.0.0.1:8091/call");
        foreach (var header in headersToSend)
        {
            request.Headers.Add(header.Key, header.Value.ToString());
        }

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode(); // Asegura que el estado sea exitoso

        var responseBody = await response.Content.ReadAsStringAsync();
        var message = $"Micro1 llamando a {responseBody}";
        await context.Response.WriteAsync(message);
    }
    catch (Exception ex)
    {
        // Maneja cualquier error
        await context.Response.WriteAsync($"Error: {ex.Message}");
    }
});

// Define la ruta /hello
app.MapGet("/hello", async context =>
{
    await context.Response.WriteAsJsonAsync(new { message = "Hello World" });
});

app.Run("http://0.0.0.0:8090"); // Define el puerto de la aplicación
