using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/call", async context =>
{
    await context.Response.WriteAsync("Micro2 responde");
});

app.Run("http://0.0.0.0:8091");
